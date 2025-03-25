using AutoMapper;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.ChatModel;
using SP25_RPSC.Data.Repositories.ChatRepository;
using SP25_RPSC.Data.Repositories.UserRepository;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Utils.CustomException;
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

        public ChatService(IUnitOfWork unitOfWork,
            IMapper mapper) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<(bool success, string senderName, string recipientName)> AddMessage(ChatMessageCreateReqModel chatMessageCreateReqModel)
        {
            var currSender = await _unitOfWork.UserRepository.GetUserById(chatMessageCreateReqModel.senderId);

            var currReceiver = await _unitOfWork.UserRepository.GetUserById(chatMessageCreateReqModel.receiverId);

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

            return (true, currSender.UserId.ToString(), currReceiver.UserId.ToString());
        }

        public async Task<List<ChatMessageViewResModel>> ViewMessageHistory(string senderId, string receiverId)
        {
            var currSender = await _unitOfWork.UserRepository.GetByIDAsync(senderId);

            var currReceiver = await _unitOfWork.UserRepository.GetByIDAsync(receiverId);

            if (currSender == null || currReceiver == null)
                throw new ApiException(System.Net.HttpStatusCode.NotFound, "Invalid senderId or receiverId");

            var messages = await _unitOfWork.ChatRepository.GetMessagesAsync(currSender.UserId, currReceiver.UserId);

            return _mapper.Map<List<ChatMessageViewResModel>>(messages);
        }

    }
}
