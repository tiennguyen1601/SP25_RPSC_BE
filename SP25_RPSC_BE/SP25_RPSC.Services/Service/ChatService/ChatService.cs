using AutoMapper;
using Newtonsoft.Json.Linq;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.ChatModel;
using SP25_RPSC.Data.Repositories.ChatRepository;
using SP25_RPSC.Data.Repositories.UserRepository;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Utils.CustomException;
using SP25_RPSC.Services.Utils.DecodeTokenHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.ChatService
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDecodeTokenHandler _decodeTokenHandler;

        public ChatService(IUnitOfWork unitOfWork,
            IMapper mapper,  IDecodeTokenHandler decodeTokenHandler) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _decodeTokenHandler = decodeTokenHandler;
        }

        public async Task<(bool success, string senderName, string recipientName)> AddMessage(ChatMessageCreateReqModel chatMessageCreateReqModel)
        {
            string senderId = chatMessageCreateReqModel.senderId;  

            string recipientId = chatMessageCreateReqModel.receiverId;

            var currSender = await _unitOfWork.UserRepository.GetByIDAsync(senderId);
            var currReceiver = await _unitOfWork.UserRepository.GetUserById(recipientId);

            if (currSender == null || currReceiver == null)
                throw new ApiException(System.Net.HttpStatusCode.NotFound, "Invalid senderId or receiverId");

            Chat chat = new Chat
            {
                Message = chatMessageCreateReqModel.message,
                SenderId = currSender.UserId,
                ReceiverId = currReceiver.UserId,
                CreateAt = DateTime.Now
            };

            await _unitOfWork.ChatRepository.Add(chat);
            await _unitOfWork.SaveAsync();

            return (true, currSender.UserId.ToString(), currReceiver.UserId.ToString());
        }


        public async Task<List<ChatMessageViewResModel>> ViewMessageHistory(string token, string receiverId)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;
            var currSender = await _unitOfWork.UserRepository.GetByIDAsync(userId);

            var currReceiver = await _unitOfWork.UserRepository.GetByIDAsync(receiverId);

            

            if (currSender == null || currReceiver == null)
                throw new ApiException(System.Net.HttpStatusCode.NotFound, "Invalid senderId or receiverId");

            var messages = await _unitOfWork.ChatRepository.GetMessagesAsync(currSender.UserId, currReceiver.UserId);

            return _mapper.Map<List<ChatMessageViewResModel>>(messages);
        }

        public async Task<List<ChatHistoryResModel>> GetHistoryByUserId(string token)
        {
            var tokenModel = _decodeTokenHandler.decode(token);
            var userId = tokenModel.userid;

            var chatHistory = await _unitOfWork.ChatRepository.Get(
                filter: c => c.SenderId == userId || c.ReceiverId == userId,
                includeProperties: "Receiver,Sender"
            );

            var latestChats = chatHistory
    .GroupBy(chat => new
    {
        User1 = string.Compare(chat.SenderId, chat.ReceiverId) < 0 ? chat.SenderId : chat.ReceiverId,
        User2 = string.Compare(chat.SenderId, chat.ReceiverId) < 0 ? chat.ReceiverId : chat.SenderId
    })
    .Select(group => group.OrderByDescending(chat => chat.CreateAt).FirstOrDefault())
    .Where(chat => chat != null)
    .ToList();


            return latestChats.Select(chat => new ChatHistoryResModel
            {
                ChatId = chat.ChatId.ToString(),
                LatestMessage = chat.Message,
                CreatedAt = chat.CreateAt ?? DateTime.UtcNow,
                Receiver = new ChatUserResModel
                {
                    Id = chat.Receiver?.UserId.ToString(),
                    Username = chat.Receiver?.FullName ?? "Unknown",
                    Avatar = chat.Receiver?.Avatar ?? "https://via.placeholder.com/40"
                },
                Sender = new ChatUserResModel
                {
                    Id = chat.Sender?.UserId.ToString(),
                    Username = chat.Sender?.FullName ?? "Unknown",
                    Avatar = chat.Sender?.Avatar ?? "https://via.placeholder.com/40"
                }
            }).ToList();

        }





    }
}
