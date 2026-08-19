using System;
using System.Collections.Generic;
using System.Text;

namespace mobileFitnes.ApiService.DataObjects
{
    public class ClassData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string Instructor { get; set; } = string.Empty;
        public DateTime StartsAt { get; set; }
        public int? MaxPeople { get; set; } = null;
        public int SignedPeople { get; set; }
        public bool YouSignedUp { get; set; }
        public ClassRepetition Repetition { get; set; } = ClassRepetition.None;

        public enum ClassRepetition
        {
            None,
            Daily,
            Weekly,
            Every2Weeks,
            Monthly,
        }
    }
}
