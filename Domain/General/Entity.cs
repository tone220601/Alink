using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.General
{
    [Serializable]
    public class Entity
    {
        [Key]
        public int Id { get; set; }
    }
}
