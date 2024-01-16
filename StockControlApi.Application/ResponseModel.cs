using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockControlApi.Application
{
    public class ResponseModel
    {
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Content { get; set; }

        public ResponseModel(int status, string message)
        {
            Status = status;
            Message = message;
        }

        public ResponseModel(int status, object content)
        {
            Status = status;
            Content = content;
        }

        public ResponseModel(int status, string message, object content)
        {
            Status = status;
            Message = message;
            Content = content;
        }

        public static ResponseModel BuildOkResponse(string message)
        {
            return new ResponseModel(200, message);
        }
        public static ResponseModel BuildOkResponse(object content)
        {
            return new ResponseModel(200, content);
        }

        public static ResponseModel BuildOkResponse(string message, object content)
        {
            return new ResponseModel(200, message, content);
        }

        public static ResponseModel BuildUnauthorizedResponse(string message)
        {
            return new ResponseModel(401, message);
        }
        public static ResponseModel BuildUnauthorizedResponse(object content)
        {
            return new ResponseModel(401, content);
        }
        public static ResponseModel BuildNotFoundResponse(string message)
        {
            return new ResponseModel(404, message);
        }

        public static ResponseModel BuildNotFoundResponse(string message, object content)
        {
            return new ResponseModel(404, message, content);
        }

        public static ResponseModel BuildConflictResponse(string message)
        {
            return new ResponseModel(409, message);
        }

        public static ResponseModel BuildConflictResponse(string message, object content)
        {
            return new ResponseModel(409, message, content);
        }

        public static ResponseModel BuildErrorResponse(string message)
        {
            return new ResponseModel(500, message);
        }
        public static ResponseModel BuildErrorResponse(object content)
        {
            return new ResponseModel(500, content);
        }
        public static ResponseModel BuildErrorResponse(string message, object content)
        {
            return new ResponseModel(500, message, content);
        }
        public static ResponseModel BuildBadRequestResponse(string message)
        {
            return new ResponseModel(400, message);
        }
    }
}
