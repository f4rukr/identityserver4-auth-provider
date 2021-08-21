using System;
using EntityFrameworkCore.Triggers;

namespace Klika.Identity.Model.Entities
{
    public interface ITrackable
    {
        public DateTime UpdatedAt { get; set; }
    }
    
    public abstract class Trackable : ITrackable
    {
        public DateTime UpdatedAt { get; set; }
     
        public Trackable()
        {
            Triggers<Trackable>.Updating += entry => entry.Entity.UpdatedAt = DateTime.Now;
        }
    }
}