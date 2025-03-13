using Azure.Storage.Blobs;
using System.Security.Cryptography;
using TestAzureCloud;

class Program
{
    static async Task Main(string[] args)
    {
        string connectionString = AppConfig.AzureStorageConnectionString;

        await SubirImagen(connectionString, @"C:\Users\SYGNO\Pictures\1.jpg");
        await SubirImagen(connectionString, @"C:\Users\SYGNO\Pictures\2.jpg");
        await SubirImagen(connectionString, @"C:\Users\SYGNO\Pictures\3.jpg");
        await SubirImagen(connectionString, @"C:\Users\SYGNO\Pictures\4.jpg");
        await SubirImagen(connectionString, @"C:\Users\SYGNO\Pictures\5.jpg");

        // Esperar a que el usuario presione una tecla antes de cerrar la consola
        Console.ReadKey();
    }

    // Método para subir una imagen
    static async Task SubirImagen(string connectionString, string filePath)
    {
        string containerName = "stdx-designs"; // Nombre del contenedor
        string fileName = Path.GetFileName(filePath);

        // Calcular el hash SHA256 del archivo local
        string fileHash = ObtenerHash(filePath);

        // Conectar con Azure Blob Storage
        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        // Verificar si el contenedor existe, si no lo crea
        await CrearContenedorSiNoExiste(containerClient);

        BlobClient blobClient = containerClient.GetBlobClient(fileName);

        // Comprobar si el blob con el mismo nombre ya existe
        if (await blobClient.ExistsAsync())
        {
            // Obtener el hash del archivo ya almacenado en el Blob
            string existingHash = await ObtenerHashExistente(blobClient);

            // Si el hash del archivo local coincide con el del archivo en el contenedor, no lo sube
            if (existingHash == fileHash)
            {
                Console.WriteLine($"El archivo {fileName} ya existe en el contenedor y es idéntico. No se subirá.");
                return;
            }
        }

        // Subir imagen
        await blobClient.UploadAsync(filePath, true);

        // Establecer el metadato con el hash del archivo
        await blobClient.SetMetadataAsync(new System.Collections.Generic.Dictionary<string, string>
        {
            { "hash", fileHash }
        });

        Console.WriteLine($"Imagen {fileName} subida correctamente a {containerName}");
    }

    // Crear contenedor si no existe
    static async Task CrearContenedorSiNoExiste(BlobContainerClient containerClient)
    {
        if (!await containerClient.ExistsAsync())
        {
            await containerClient.CreateAsync();
            Console.WriteLine("Contenedor creado exitosamente.");
        }
        else
        {
            Console.WriteLine("El contenedor ya existe.");
        }
    }

    // Obtener el hash SHA256 del archivo
    static string ObtenerHash(string filePath)
    {
        using (var sha256 = SHA256.Create())
        using (var stream = File.OpenRead(filePath))
        {
            byte[] hashBytes = sha256.ComputeHash(stream);
            return BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
        }
    }

    // Obtener el hash del archivo ya almacenado en el Blob (metadato)
    static async Task<string> ObtenerHashExistente(BlobClient blobClient)
    {
        var blobProperties = await blobClient.GetPropertiesAsync();
        if (blobProperties.Value.Metadata.ContainsKey("hash"))
        {
            return blobProperties.Value.Metadata["hash"];
        }

        // Si no hay hash, devuelve una cadena vacía
        return string.Empty;
    }
}
