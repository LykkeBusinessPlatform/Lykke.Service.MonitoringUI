var Requests = (function () {
    function Requests() {
    }
    Requests.doRequest = function (o) {
        var _this = this;
        if (!o.url) {
            throw "url is empty";
        }
        o.params = o.formData
            ? o.formData
            : o.formId
                ? $(o.formId).serialize()
                : o.params;
        if (o.putToLast)
            this.setRefreshUrl(o);
        if (o.partialUpdate)
            this.setUpdateInfo(o);
        if (o.onStart)
            o.onStart();
        this.blockOnRequest();
        if (o.divResult) {
            $(o.divResult).fadeOut(200, function () { return _this.peformRequest(o); });
        }
        else
            this.peformRequest(o);
    };
    Requests.setRefreshAsReloadPage = function () {
        this.reloadAction.reloadPage = true;
    };
    Requests.setRefreshUrl = function (r) {
        this.reloadAction.reloadPage = false;
        this.reloadAction.div = r.divResult;
        this.reloadAction.url = r.url;
        this.reloadAction.data = r.params;
        this.reloadAction.showLoading = r.showLoading;
    };
    Requests.setUpdateInfo = function (r) {
        this.reloadAction.reloadPage = false;
        this.reloadAction.div = r.divResult;
        this.reloadAction.url = r.requestUrl;
        this.reloadAction.data = r.requestParams;
        this.reloadAction.showLoading = r.showLoading;
    };
    Requests.showLoadingInDiv = function (id) {
        $(id).html('<div style="text-align:center; margin-top:20px;"><img src="/images/loading-pa.gif"/></div>');
        $(id).show();
    };
    Requests.peformRequest = function (o) {
        var _this = this;
        if (o.onShowLoading)
            o.onShowLoading();
        else {
            if (o.showLoading !== false && o.divResult) {
                this.showLoadingInDiv(o.divResult);
            }
        }
        var options = o.formData
            ? { url: o.url, type: 'POST', data: o.params, processData: false, contentType: false }
            : { url: o.url, type: 'POST', data: o.params };
        $.ajax(options)
            .then(function (result, statusText, jqXHR) {
            if (o.onHideLoading)
                o.onHideLoading();
            if (o.onSuccess)
                try {
                    var object = JSON.parse(jqXHR.responseText);
                    o.onSuccess(jqXHR.status, object);
                }
                catch (e) {
                    o.onSuccess(jqXHR.status);
                }
            if (o.onFinish)
                o.onFinish();
            if (o.divResult)
                $(o.divResult).hide();
            _this.unBlockOnRequest();
            _this.requestOkHandler(o, result);
        })
            .fail(function (jqXhr) {
            if (o.onHideLoading)
                o.onHideLoading();
            if (o.onError) {
                try {
                    var object = JSON.parse(jqXhr.responseText);
                    if (object.errors) {
                        object.message = object.message ? object.message : '';
                        for (var i in object.errors) {
                            object.message += " " + object.errors[i];
                        }
                    }
                    o.onError(object.message);
                }
                catch (e) {
                    o.onError(undefined);
                }
            }
            if (o.onFinish)
                o.onFinish();
            _this.unBlockOnRequest();
            _this.requestFailHandle(o, jqXhr.responseText, jqXhr);
        });
    };
    Requests.requestOkHandler = function (o, result) {
        var _this = this;
        if (result.status === 'Fail') {
            if (result.divError)
                o.divError = result.divError;
            o.placement = result.placement;
            this.requestFailHandle(o, result.msg);
            return;
        }
        if (result.status === "Reload") {
            this.ui.hideDialog();
            if (this.reloadAction.reloadPage)
                location.reload();
            else
                this.doRequest({ url: this.reloadAction.url, params: this.reloadAction.data, divResult: this.reloadAction.div, showLoading: this.reloadAction.showLoading });
            return;
        }
        if (result.refreshUrl) {
            this.ui.hideDialog();
            this.doRequest({ url: result.refreshUrl, params: result.prms, divResult: result.div, putToHistory: result.putToHistory, onResultRenderFinish: o.onResultRenderFinish });
            return;
        }
        if (result.inputId) {
            $(result.inputId).val(result.text);
            return;
        }
        if (result.status === "Request") {
            this.doRequest({ url: result.url, params: result.prms, putToHistory: result.putToHistory, onResultRenderFinish: o.onResultRenderFinish });
            return;
        }
        if (result.status === "refreshLast") {
            this.last();
            return;
        }
        if (result.status === "OkAndNothing") {
            if (o.callBack)
                o.callBack(result);
            return;
        }
        if (result.status === "ShowDialog") {
            this.ui.hideDialog(function () {
                _this.ui.showDialog(result.url, result.prms);
            });
        }
        if (result.status === "Redirect") {
            if (result.Params) {
                window.location = (result.Url + "?" + result.Params);
            }
            else
                window.location = result.Url;
            return;
        }
        if (o.divResult) {
            if (o.replaceDiv) {
                $(o.divResult).replaceWith(result);
            }
            else {
                $(o.divResult).html(result);
                $(o.divResult).fadeIn(200, function () {
                    _this.pageManager.ressize();
                });
            }
            if (o.onResultRenderFinish)
                o.onResultRenderFinish();
            this.pageManager.ressize();
            this.ui.initFocus();
        }
    };
    Requests.handleErrorReq = function (o, result) {
        if (result.status === "Fail")
            this.ui.showError(result.divError, result.Result, undefined, result.Placement);
        else if (result.responseText && o.divError)
            this.ui.showError(o.divError, result.responseText);
    };
    Requests.requestFailHandle = function (o, text, jqXhr) {
        if (jqXhr && jqXhr.status === 403) {
            if (o.divResult) {
                $(o.divResult).html('<div class="alert alert-danger" style="text-align:center; margin-top:20px;"><h1>Access denied</h1>You don\'t have an access to visit the ' + o.url + ' page.</div>');
            }
        }
        else {
            this.ui.showError(o.divError, text, undefined, o.placement);
        }
    };
    Requests.blockOnRequest = function () {
        $('.disableOnRequest').each(function () {
            $(this).attr('disabled', 'true');
        });
        $('.hideOnRequest').each(function () {
            $(this).css('display', 'none');
        });
        $('.showOnRequest').each(function () {
            $(this).css('display', 'inline');
        });
    };
    Requests.unBlockOnRequest = function () {
        $('.disableOnRequest').each(function () {
            $(this).removeAttr('disabled');
        });
        $('.hideOnRequest').each(function () {
            $(this).css('display', 'inline');
        });
        $('.showOnRequest').each(function () {
            $(this).css('display', 'none');
        });
    };
    return Requests;
}());
Requests.reloadAction = {
    reloadPage: true,
    url: "",
    data: undefined,
    div: "",
    showLoading: true
};
var requests = Requests;
//# sourceMappingURL=requests.js.map