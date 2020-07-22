using API_WMParking.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace API_WMParking.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [Produces(MediaTypeNames.Application.Json)]
    public class ParkingLotController : ControllerBase
    {
        private static readonly HashSet<VehicleRegistration> registrations = new HashSet<VehicleRegistration>();

        [Route("registrations")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<VehicleRegistration> Registrations()
        {
            return registrations;
        }

        [Route("vehicle/register/{vehicleId}")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult RegisterVehicle([Required] string vehicleId)
        {
            if (!registrations.Add(new VehicleRegistration(vehicleId, DateTime.Now)))
            {
                throw new InvalidOperationException("vehicle already registered");
            }
            
            return Ok();
        }

        [Route("vehicle/is-registered/{vehicleId}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public bool IsRegistered([Required] string vehicleId)
        {
            return registrations.Contains(new VehicleRegistration(vehicleId, default));
        }

        [Route("vehicle/time-spent/{vehicleId}/{timeUnit}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public double TimeSpent([Required] string vehicleId, TimeUnits timeUnit = TimeUnits.Minute)
        {
            DateTime registrationTime = ExistentRegistration(vehicleId).Time;
            TimeSpan timeLength = DateTime.Now - registrationTime;

            switch (timeUnit)
            {
                case TimeUnits.Hour: return timeLength.TotalHours;
                default: return timeLength.TotalMinutes;
            }
        }

        [Route("vehicle/current-parking-cost/{vehicleId}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public decimal CurrentParkingCost([Required] string vehicleId)
        {
            double hoursSpent = TimeSpent(vehicleId, TimeUnits.Hour);

            decimal costPerHour = 2;

            return (decimal)hoursSpent * costPerHour;
        }

        [Route("vehicle/deregister/{vehicleId}")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult DeregisterVehicle([Required] string vehicleId)
        {
            registrations.Remove(ExistentRegistration(vehicleId));

            return Ok();
        }

        private VehicleRegistration ExistentRegistration(string vehicleId)
        {
            VehicleRegistration value;

            if (!registrations.TryGetValue(new VehicleRegistration(vehicleId, default), out value))
            {
                throw new InvalidOperationException("vehicle has not been previously registered");
            }

            return value;
        }
    }
}