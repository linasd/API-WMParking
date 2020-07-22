using System;
using System.Diagnostics.CodeAnalysis;

namespace API_WMParking.Types
{
    public class VehicleRegistration : IEquatable<VehicleRegistration>
    {
        public string VehicleId { get; private set; }
        
        public DateTime Time { get; private set; }

        public VehicleRegistration(string vehicleId, DateTime time)
        {
            VehicleId = vehicleId;
            Time = time;
        }

        public bool Equals([AllowNull] VehicleRegistration other)
        {
            return other != null && VehicleId == other.VehicleId;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as VehicleRegistration);
        }

        public override int GetHashCode()
        {
            return VehicleId.GetHashCode();
        }
    }
}