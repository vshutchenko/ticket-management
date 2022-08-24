﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TicketManagement.UserApi.Services.Validation;

namespace TicketManagement.UserApi.Filters
{
    public class ValidationExceptionFilter : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception.GetType() != typeof(ValidationException))
            {
                return;
            }

            context.ExceptionHandled = true;

            context.Result = new BadRequestObjectResult(new { error = context.Exception.Message });
        }
    }
}
