using System;
using System.Linq;
using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SurveyUs.Web.Abstractions
{
    public abstract class BaseController<T> : Controller
    {
        private IMediator _mediatorInstance;
        private ILogger<T> _loggerInstance;
        private IViewRenderService _viewRenderInstance;
        private IMapper _mapperInstance;
        private INotyfService _notifyInstance;
        protected INotyfService _notify => _notifyInstance ??= HttpContext.RequestServices.GetService<INotyfService>();

        protected IMediator _mediator => _mediatorInstance ??= HttpContext.RequestServices.GetService<IMediator>();
        protected ILogger<T> _logger => _loggerInstance ??= HttpContext.RequestServices.GetService<ILogger<T>>();
        protected IViewRenderService _viewRenderer => _viewRenderInstance ??= HttpContext.RequestServices.GetService<IViewRenderService>();
        protected IMapper _mapper => _mapperInstance ??= HttpContext.RequestServices.GetService<IMapper>();

        protected bool IsValidModel<T>(object model, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                Type validatorType = typeof(T);

                if (!validatorType.BaseType.IsGenericType)
                    return true;

                if (validatorType.BaseType.GetGenericTypeDefinition() != typeof(AbstractValidator<>))
                {
                    //resp = CreateExceptionActionResult(ErrorBaseCode.Validator.Error_Validator_Invalid_Type.errorMessage);
                    return false;
                }

                var obj = System.Activator.CreateInstance(validatorType);

                var validateInfo = validatorType.GetMethod("Validate", new Type[] { model.GetType() });

                ValidationResult result = validateInfo?.Invoke(obj, new object[] { model }) as ValidationResult;

                if (result == null)
                {
                    //resp = CreateErrorActionResult(ErrorBaseCode.Validator.Error_Validator_Failed);
                    return false;
                }

                if (!result.IsValid)
                {
                    errorMessage = System.String.Join(", ", result.Errors.Select(x => x.ErrorMessage).Distinct());
                    var eExtMsg = System.String.Join(", ", result.Errors.Select(x => x.ErrorCode).Distinct());

                    //resp = CreateErrorActionResult(ErrorBaseCode.Validator.Error_Validator_Failed, errMessage: eMsg, errExtMessage: eExtMsg);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                //resp = CreateTryCatchExceptionActionResult(ErrorBaseCode.Validator.Error_Validator_Exception.errorMessage, exception: ex);
            }

            return false;
        }
    }
}