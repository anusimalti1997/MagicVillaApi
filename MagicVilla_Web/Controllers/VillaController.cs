﻿using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Response;
using MagicVilla_Web.Models.Villa.Dto;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;
        public VillaController(IVillaService villaService,IMapper mapper)
        {
            _mapper = mapper;
            _villaService = villaService;
        }

        public async Task<IActionResult> IndexVilla()
        {
            List<VillaDTO> list = new();

            var response = await _villaService.GetAllAsync<ApiResponse>();

            if(response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));
            }

            return View(list);
        }

        public async Task<IActionResult> CreateVilla()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVilla(VillaCreateDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService.CreateAsync<ApiResponse>(model);
                if(response!= null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexVilla));
                }
            }
                return View(model);
        }

        public async Task<IActionResult> UpdateVilla()
        {

            return View();
        }
    }
}
