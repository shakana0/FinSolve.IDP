using FinSolve.IDP.Domain.Enums;
using FinSolve.IDP.Domain.ValueObjects;

namespace FinSolve.IDP.Domain.Entities
{
    public class Document
    {
        public DocumentId Id { get; }
        public string FileName { get; }
        public string BlobPath { get; }
        public DocumentType Type { get; }
        public Hash Hash { get; } //idempotency
        public Metadata Metadata { get; }

        public Document(DocumentId id, string fileName, string blobPath, DocumentType type, Hash hash, Metadata metadata)
        {
            Id = id;
            FileName = fileName;
            BlobPath = blobPath;
            Type = type;
            Hash = hash;
            Metadata = metadata;
        }
    }

}
