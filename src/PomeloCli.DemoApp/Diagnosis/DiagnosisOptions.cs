using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PomeloCli.DemoApp.Diagnosis {
    public class DiagnosisOptions : IValidatableObject {
        public Boolean Enable { get; set; }
        
        public String Url { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (Enable && Url != null && Uri.IsWellFormedUriString(Url, UriKind.RelativeOrAbsolute) == false) {
                yield return new ValidationResult("Url missing or invalid", new[] { nameof(Url) });
            }
        }
    }
}