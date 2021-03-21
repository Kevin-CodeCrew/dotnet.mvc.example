

namespace test_mvc_webapp.Models
{
public class EmailModel {  
        public EmailModel(string to, string subject, string message, bool isBodyHtml) {  
            To = to;  
            Subject = subject;  
            Message = message;  
            IsBodyHtml = isBodyHtml;  
        }  
        public string To {  
            get;  
        }  
        public string Subject {  
            get;  
        }  
        public string Message {  
            get;  
        }  
        public bool IsBodyHtml {  
            get;  
        }  
    }  
}