using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Lykke.Service.MonitoringUI.Domain;
using Lykke.Service.MonitoringUI.Domain.Services;
using Lykke.Service.MonitoringUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.MonitoringUI.Controllers
{
    public class MonitoringUiController : Controller
    {
        private readonly IMonitoringServiceCallerService _monitoringServiceCallerService;
        private readonly IHttpClientFactory _httpClientFactory;

        public MonitoringUiController(
            IMonitoringServiceCallerService monitoringServiceCallerService,
            IHttpClientFactory httpClientFactory)
        {
            _monitoringServiceCallerService = monitoringServiceCallerService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UrlError()
        {
            return View(
                new ErrorViewModel
                {
                    Caption = "Url Error",
                    Width = "900px"
                });
        }

        [HttpPost]
        public async Task<ActionResult> GetData()
        {
            IEnumerable<MonitoringRecordExtended> monitoringObjectModels;

            try
            {
                monitoringObjectModels = await _monitoringServiceCallerService.GetAllAsync();
            }
            catch
            {
                monitoringObjectModels = new List<MonitoringRecordExtended>();
            }

            var vm = new SrvMonitoringDataViewModel
            {
                MonitoringServiceRecords = monitoringObjectModels
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> MuteMonitorDialog(string serviceName)
        {
            var record = await _monitoringServiceCallerService.GetServiceAsync(serviceName);
            int minutes = record?.SkipUntil != null ? (record.SkipUntil - DateTime.UtcNow).Value.Minutes : 0;
            var viewModel = new MuteMonitorDialogViewModel
            {
                Minutes = minutes,
                ServiceName = serviceName,
                Caption = "Mute",
                Width = "900px"
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> MuteMonitor(MuteMonitorModel model)
        {
            await _monitoringServiceCallerService.MuteServiceAsync(model.ServiceName, model.Minutes);

            return JsonRequestResult("#pamain", Url.Action("Index"));
        }

        [HttpPost]
        public ActionResult UnMuteMonitorDialog(string serviceName)
        {
            var viewModel = new UnMuteMonitorDialogViewModel
            {
                ServiceName = serviceName,
                Caption = "Unmute",
                Width = "900px"
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> UnMuteMonitor(UnMuteMonitorModel model)
        {
            await _monitoringServiceCallerService.UnMuteServiceAsync(model.ServiceName);

            return JsonRequestResult("#pamain", Url.Action("Index"));
        }

        [HttpPost]
        public ActionResult AddMonitorDialog()
        {
            var viewModel = new AddMonitorDialogViewModel
            {
                Caption = "Add monitoring object with url",
                Width = "900px"
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> AddMonitor(AddMonitorModel model)
        {
            try
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    var response = await httpClient.GetAsync(new Uri(model.Url));
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception)
            {
                return JsonResultShowDialog(Url.Action("UrlError"));
            }
            await _monitoringServiceCallerService.AddUrlToMonitoringAsync(model.ServiceName, model.Url);

            return JsonRequestResult("#pamain", Url.Action("Index"));
        }

        [HttpPost]
        public ActionResult RemoveMonitorDialog(string serviceName)
        {
            var viewModel = new RemoveMonitorDialogViewModel
            {
                ServiceName = serviceName,
                Caption = "Remove monitoring object?",
                Width = "900px"
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> RemoveMonitor(RemoveMonitorModel model)
        {
            await _monitoringServiceCallerService.RemoveUrlFromMonitoring(model.ServiceName);

            return JsonRequestResult("#pamain", Url.Action("Index"));
        }

        private static JsonResult JsonRequestResult(string div, string url)
        {
            return new JsonResult(new { div, refreshUrl = url, showLoading = true });
        }

        private static JsonResult JsonResultShowDialog(string url)
        {
            return new JsonResult(new { status = "ShowDialog", url });
        }
    }
}
