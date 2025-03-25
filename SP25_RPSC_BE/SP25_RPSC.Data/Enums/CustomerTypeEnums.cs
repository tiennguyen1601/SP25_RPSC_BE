using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CustomerTypeEnums
{
    [EnumMember(Value = "Student")]
    Student = 0,

    [EnumMember(Value = "Worker")]
    Worker = 1
}



