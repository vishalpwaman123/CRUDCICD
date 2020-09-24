using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLayer.Exceptions
{
    public class AdminExceptions : Exception
    {

        /// <summary>
        /// Enum For Exception types.
        /// </summary>
        public enum ExceptionType
        {
            INVALID_ROLE_EXCEPTION,
            NULL_EXCEPTION,
            EMPTY_EXCEPTION,
            INVALID_MATCH_PASSWORD_EXCEPTION,
            INVALID_EMAIL_IDENTITY,
            INVALID_PASSWORD_IDENTITY,
            ERROR_ON_ROLE_DELETION
        }

        /// <summary>
        /// Exception type Reference.
        /// </summary>
        ExceptionType type;

        /// <summary>
        /// Constrcutor For Setting Exception Type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public AdminExceptions(AdminExceptions.ExceptionType type, string message) : base(message)
        {
            this.type = type;
        }


    }
}
