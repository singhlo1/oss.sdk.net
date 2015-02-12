using System;
using Silanis.ESL.SDK;
using Silanis.ESL.SDK.Builder;
using System.IO;

namespace SDK.Examples
{
    public class FieldInjectionExample : SDKSample {
        public static void Main (string[] args)
        {
            new FieldInjectionExample(Props.GetInstance()).Run();
        }

        private string email1;
        private Stream fileStream1;

        public FieldInjectionExample( Props props ) : this(props.Get("api.key"), props.Get("api.url"), props.Get("1.email")) {
        }

        public FieldInjectionExample( String apiKey, String apiUrl, String email1 ) : base( apiKey, apiUrl ) {
            this.email1 = email1;
            this.fileStream1 = File.OpenRead(new FileInfo(Directory.GetCurrentDirectory() + "/src/document-with-fields.pdf").FullName);
        }

        override public void Execute()
        {
            DocumentPackage superDuperPackage = PackageBuilder.NewPackageNamed( "FieldInjectionExample " + DateTime.Now )
                .WithSettings(DocumentPackageSettingsBuilder.NewDocumentPackageSettings().WithInPerson())
                .WithSigner( SignerBuilder.NewSignerWithEmail( email1 )
                            .WithFirstName( "John" )
                            .WithLastName( "Smith" ) )
                    .WithDocument( DocumentBuilder.NewDocumentNamed( "First Document" )
                                  .FromStream(fileStream1, DocumentType.PDF)
                                  .EnableExtraction()
                                  .WithSignature( SignatureBuilder.SignatureFor( email1 )
                                   .WithPositionExtracted() )
                                  .WithInjectedField( FieldBuilder.TextField()
                                       .WithPositionExtracted()
                                       .WithId( "AGENT_SIG_1" )
                                       .WithName( "AGENT_SIG_1" )
                                       .WithValue( "Test Value" ) )
                                  .WithInjectedField( FieldBuilder.SignatureDate()
                                       .WithPositionExtracted() ) )
                    .Build();

            packageId = eslClient.CreatePackage( superDuperPackage );
            eslClient.SendPackage( PackageId );
        }
    }
}

