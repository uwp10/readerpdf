// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PdfService.cs" company="vlady-mix">
//    Fabricio Altamirano  2016
//  </copyright>
//  <summary>
//    The definition of  PdfService.cs
//  </summary>
//  --------------------------------------------------------------------------------------------------------------------
namespace PdfService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Threading.Tasks;
    using Windows.Data.Pdf;
    using Windows.Graphics.Imaging;
    using Windows.Storage;
    using Windows.Storage.Streams;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    ///     The pdf service.
    /// </summary>
    public class PdfService : IPdfService
    {
        #region Private Fields

        /// <summary>
        ///     The pages.
        /// </summary>
      

        /// <summary>
        ///     The pdf document.
        /// </summary>
        private PdfDocument pdfDocument;

        /// <summary>
        ///     The pdf pages.
        /// </summary>
      

        private List<PDFPage> PDFpages;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PdfService" /> class.
        /// </summary>
        public PdfService()
        {
            this.Resolution = 2;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the size document.
        /// </summary>
        public double SizeDocument { get; set; }

        #endregion

        #region Interface Members

        /// <summary>
        ///     Gets or sets the current page.
        /// </summary>
        public PdfPage CurrentPage { get; set; }

        /// <summary>
        ///     Gets or sets the resolution.
        /// </summary>
        public uint Resolution { get; set; }

        /// <summary>
        ///     Gets or sets the total pages.
        /// </summary>
        public uint TotalPages { get; set; }

        /// <summary>
        /// The get page.
        /// </summary>
        /// <param name="numPage">
        /// The num page.
        /// </param>
        /// <returns>
        /// The <see cref="BitmapImage"/>.
        /// </returns>
        public BitmapImage GetPageAsBitmapImage(int numPage)
        {
            return this.PDFpages[numPage].ImagePreview;
        }

        /// <summary>
        /// The get pages.
        /// </summary>
        public List<BitmapImage> GetPagesAsBitmapImages()
        {
            return this.PDFpages.Select(page => page.ImagePreview).ToList();
        }

        /// <summary>
        /// The get pdf page.
        /// </summary>
        /// <param name="numPage">
        /// The num page.
        /// </param>
        /// <returns>
        /// The <see cref="PdfPage"/>.
        /// </returns>
        public PdfPage GetPdfPage(int numPage)
        {
            return this.PDFpages[numPage].PdfPage;
        }

        public List<PDFPage> GetPDFPages()
        {
            return this.PDFpages;

        }

        /// <summary>
        /// The get pdf pages.
        /// </summary>
        public List<PdfPage> GetPdfPages()
        {
            return this.PDFpages.Select(item=>item.PdfPage).ToList();
        }

        /// <summary>
        /// The load from local file.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task LoadFromLocalFile()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
            };

            picker.FileTypeFilter.Add(".pdf");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                await this.LoadFromStorageFile(file);
            }
        }

        /// <summary>
        /// The load file.
        /// </summary>
        /// <param name="pdfFile">
        /// The pdf file.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task LoadFromStorageFile(IStorageFile pdfFile)
        {
            this.PDFpages = new List<PDFPage>();

            this.pdfDocument = await PdfDocument.LoadFromFileAsync(pdfFile);
            this.TotalPages = this.pdfDocument.PageCount;


            for (uint pageNum = 0; pageNum < this.pdfDocument.PageCount; pageNum++)
            {
                var page = this.pdfDocument.GetPage(pageNum);

                var stream = new InMemoryRandomAccessStream();

                var pdfPageRenderOptions = new PdfPageRenderOptions
                {
                    DestinationHeight = (uint)page.Size.Height * this.Resolution
                };

                await page.RenderToStreamAsync(stream, pdfPageRenderOptions);

                var source = new BitmapImage();
                source.SetSource(stream);

                // Image pdfPage = new Image();
                // pdfPage.HorizontalAlignment = HorizontalAlignment.Center;
                // pdfPage.VerticalAlignment = VerticalAlignment.Center;
                // pdfPage.Height = page.Size.Height;
                // pdfPage.Width = page.Size.Width;
                // pdfPage.Margin = new Thickness(0, 0, 0, 5);
                // pdfPage.Source = source;

                this.PDFpages.Add(new PDFPage {ImagePreview =  source, PdfPage = page});
               
            }
        }

        public async Task LoadFromUrl(Uri uriLocation)
        {
             this.PDFpages = new List<PDFPage>();
            var streamPdf = await this.GetStreamFromUrl(uriLocation);

            // Create a .NET memory stream.
            var memStream = new MemoryStream();

            // Convert the stream to the memory stream, because a memory stream supports seeking.
            await streamPdf.CopyToAsync(memStream);

            // Set the start position.
            memStream.Position = 0;

            this.pdfDocument = await PdfDocument.LoadFromStreamAsync(memStream.AsRandomAccessStream());
            this.TotalPages = this.pdfDocument.PageCount;


            for (uint pageNum = 0; pageNum < this.pdfDocument.PageCount; pageNum++)
            {
                var page = this.pdfDocument.GetPage(pageNum);

                var stream = new InMemoryRandomAccessStream();

                await page.RenderToStreamAsync(stream);

                var source = new BitmapImage();
                source.SetSource(stream);

                this.PDFpages.Add(new PDFPage { ImagePreview = source, PdfPage = page });
            }
        }

        /// <summary>
        /// The save page to jpg.
        /// </summary>
        /// <param name="numPage">
        /// The num page.
        /// </param>
        /// <param name="outputFile">
        /// The output file.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task SavePageToJpg(uint numPage, IStorageFile outputFile)
        {
            var page = this.pdfDocument.GetPage(numPage);

            var stream = new InMemoryRandomAccessStream();

            var pdfPageRenderOptions = new PdfPageRenderOptions
            {
                DestinationHeight = (uint)page.Size.Height * this.Resolution
            };

            await page.RenderToStreamAsync(stream, pdfPageRenderOptions);

            // initialize with 1,1 to get the current size of the image
            var writeableBmp = new WriteableBitmap(1, 1);
            writeableBmp.SetSource(stream);

            // and create it again because otherwise the WB isn't fully initialized and decoding
            // results in a IndexOutOfRange
            writeableBmp = new WriteableBitmap(writeableBmp.PixelWidth, writeableBmp.PixelHeight);

            stream.Seek(0);
            writeableBmp.SetSource(stream);

            await this.SaveToFile(writeableBmp, outputFile);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// The save to file.
        /// </summary>
        /// <param name="writeableBitmap">
        /// The writeable bitmap.
        /// </param>
        /// <param name="outputFile">
        /// The output file.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task SaveToFile(WriteableBitmap writeableBitmap, IStorageFile outputFile)
        {
            try
            {
                var stream = writeableBitmap.PixelBuffer.AsStream();

                var pixels = new byte[(uint)stream.Length];
                await stream.ReadAsync(pixels, 0, pixels.Length);

                using (var streamOut = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, streamOut);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (uint)writeableBitmap.PixelWidth, (uint)writeableBitmap.PixelHeight, 96, 96, pixels);

                    await encoder.FlushAsync();

                    using (var outputStream = streamOut.GetOutputStreamAt(0))
                    {
                        await outputStream.FlushAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private Task<Stream> GetStreamFromUrl(Uri url)
        {
            var tcs = new TaskCompletionSource<Stream>();

            var client = new HttpClient();

            var response = client.GetStreamAsync(url);

            response.ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    tcs.TrySetException(response.Exception.GetBaseException());
                }
                else if (task.Result != null)
                {
                    tcs.TrySetResult(task.Result);
                }

            });

            return tcs.Task;
        }



        #endregion
    }
}