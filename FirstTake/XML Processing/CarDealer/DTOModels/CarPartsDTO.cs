using System.Xml.Serialization;

namespace CarDealer.DTOModels
{
    public class CarPartsDTO
    {

        // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class cars
        {

            private carsCar carField;

            /// <remarks/>
            public carsCar car
            {
                get
                {
                    return this.carField;
                }
                set
                {
                    this.carField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [XmlType("car")]
        public partial class carsCar
        {

            private carsCarPart[] partsField;

            private string makeField;

            private string modelField;

            private long travelleddistanceField;

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayItemAttribute("part", IsNullable = false)]
            public carsCarPart[] parts
            {
                get
                {
                    return this.partsField;
                }
                set
                {
                    this.partsField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string make
            {
                get
                {
                    return this.makeField;
                }
                set
                {
                    this.makeField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string model
            {
                get
                {
                    return this.modelField;
                }
                set
                {
                    this.modelField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute("travelled-distance")]
            public long travelleddistance
            {
                get
                {
                    return this.travelleddistanceField;
                }
                set
                {
                    this.travelleddistanceField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class carsCarPart
        {

            private string nameField;

            private decimal priceField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string name
            {
                get
                {
                    return this.nameField;
                }
                set
                {
                    this.nameField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public decimal price
            {
                get
                {
                    return this.priceField;
                }
                set
                {
                    this.priceField = value;
                }
            }
        }


    }
}
