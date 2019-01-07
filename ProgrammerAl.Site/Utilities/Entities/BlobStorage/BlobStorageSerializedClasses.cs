using System;
using System.Collections.Generic;
using System.Text;

namespace ProgrammerAl.Site.Utilities.Entities.BlobStorage
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class EnumerationResults
    {

        private string prefixField;

        private EnumerationResultsBlob[] blobsField;

        private object nextMarkerField;

        private string containerNameField;

        /// <remarks/>
        public string Prefix
        {
            get
            {
                return this.prefixField;
            }
            set
            {
                this.prefixField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Blob", IsNullable = false)]
        public EnumerationResultsBlob[] Blobs
        {
            get
            {
                return this.blobsField;
            }
            set
            {
                this.blobsField = value;
            }
        }

        /// <remarks/>
        public object NextMarker
        {
            get
            {
                return this.nextMarkerField;
            }
            set
            {
                this.nextMarkerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ContainerName
        {
            get
            {
                return this.containerNameField;
            }
            set
            {
                this.containerNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnumerationResultsBlob
    {

        private string nameField;

        private string urlField;

        private string lastModifiedField;

        private string etagField;

        private ushort sizeField;

        private string contentTypeField;

        private object contentEncodingField;

        private object contentLanguageField;

        /// <remarks/>
        public string Name
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
        public string Url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        public string LastModified
        {
            get
            {
                return this.lastModifiedField;
            }
            set
            {
                this.lastModifiedField = value;
            }
        }

        /// <remarks/>
        public string Etag
        {
            get
            {
                return this.etagField;
            }
            set
            {
                this.etagField = value;
            }
        }

        /// <remarks/>
        public ushort Size
        {
            get
            {
                return this.sizeField;
            }
            set
            {
                this.sizeField = value;
            }
        }

        /// <remarks/>
        public string ContentType
        {
            get
            {
                return this.contentTypeField;
            }
            set
            {
                this.contentTypeField = value;
            }
        }

        /// <remarks/>
        public object ContentEncoding
        {
            get
            {
                return this.contentEncodingField;
            }
            set
            {
                this.contentEncodingField = value;
            }
        }

        /// <remarks/>
        public object ContentLanguage
        {
            get
            {
                return this.contentLanguageField;
            }
            set
            {
                this.contentLanguageField = value;
            }
        }
    }


}
