using JetBrains.Annotations;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lykke.Service.MonitoringUI
{
    [UsedImplicitly]
    public static class PartialHelpers
    {
        public static IHtmlContent RenderSaveCancelButtonPair(this IHtmlHelper helper, string url, string formId, string savePhrase = null, string onFinishJs = null, string onValidateJs = null, string saveButtonId = null, string onResultRenderFinish = null, string onSuccess = null, string onError = null, string callback = null)
        {
            return helper.Partial(
                "~/Views/Helpers/RenderSaveCancelButtonPair.cshtml",
                new SaveCancelButtonPairModel
                {
                    Url = url,
                    FormId = formId,
                    SavePhrase = savePhrase,
                    OnFinish = onFinishJs,
                    OnValidate = onValidateJs,
                    SaveButtonId = saveButtonId,
                    OnResultRenderFinish = onResultRenderFinish,
                    OnSuccess = onSuccess,
                    OnError = onError,
                    Callback = callback
                });
        }

        public static IHtmlContent RenderInputWithLabelOnTop(this IHtmlHelper helper, string name, string caption, string value = null, string placeHolder = null, string type = null, bool lg = true, bool focused = false)
        {
            return helper.Partial(
                "~/Views/Helpers/RenderInputWithLabelOnTop.cshtml",
                new InputWithLabelModel
                {
                    Name = name,
                    Caption = caption,
                    Value = value,
                    Placeholder = placeHolder,
                    Type = type,
                    Lg = lg,
                    Focused = focused
                });
        }
    }

    public class SaveCancelButtonPairModel
    {
        public string Url { get; set; }
        public string FormId { get; set; }
        public string SavePhrase { get; set; }
        public string OnFinish { get; set; }
        public string OnValidate { get; set; }
        public string OnSuccess { get; set; }
        public string Callback { get; set; }
        public string OnError { get; set; }
        public string SaveButtonId { get; set; }
        public string OnResultRenderFinish { get; set; }
    }

    public class InputWithLabelModel
    {
        public string Name { get; set; }
        public string Caption { get; set; }
        public string Value { get; set; }
        public string Placeholder { get; set; }
        public string Type { get; set; }
        public bool Lg { get; set; }
        public bool Focused { get; set; }
    }
}
