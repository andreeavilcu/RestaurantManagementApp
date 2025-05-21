using RestaurantManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RestaurantManagementApp.Converters
{
    public class FirstImagePathConverter : IValueConverter
    {
        private static BitmapImage _defaultImage;

        static FirstImagePathConverter()
        {
            try
            {
                _defaultImage = new BitmapImage(new Uri("pack://application:,,,/Images/noimage.png", UriKind.Absolute));
            }
            catch
            {
                //_defaultImage = CreateBlankImage(150, 150);
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"==== IMAGE CONVERTER ====");
                System.Diagnostics.Debug.WriteLine($"Converting image. Value type: {value?.GetType().Name ?? "null"}");

                // Case 1: Collection of strings
                if (value is ObservableCollection<string> imageStrings && imageStrings.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Found collection of strings with {imageStrings.Count} items");

                    foreach (var path in imageStrings)
                    {
                        System.Diagnostics.Debug.WriteLine($"Checking path: {path}");
                        var image = LoadImageSafely(path);
                        if (image != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"Successfully loaded image for path: {path}");
                            return image;
                        }
                    }
                }

                // Case 2: Collection of ProductImage objects
                if (value is ObservableCollection<ProductImage> imageObjects && imageObjects.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Found collection of ProductImage with {imageObjects.Count} items");

                    foreach (var imgObj in imageObjects)
                    {
                        System.Diagnostics.Debug.WriteLine($"Checking ProductImage path: {imgObj.ImagePath}");
                        var image = LoadImageSafely(imgObj.ImagePath);
                        if (image != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"Successfully loaded image for ProductImage path: {imgObj.ImagePath}");
                            return image;
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine("No valid image found, returning default");
                return _defaultImage;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in converter: {ex.Message}");
                return _defaultImage;
            }
        }

        private BitmapImage LoadImageSafely(string originalPath)
        {
            if (string.IsNullOrEmpty(originalPath))
            {
                System.Diagnostics.Debug.WriteLine("Path is null or empty");
                return null;
            }

            System.Diagnostics.Debug.WriteLine($"Trying to load image from: {originalPath}");

            // Obține numele fișierului
            string fileName = Path.GetFileName(originalPath);
            System.Diagnostics.Debug.WriteLine($"File name: {fileName}");

            // Lista cu locații de verificat
            List<string> pathsToTry = new List<string>();

            // 1. Calea originală
            pathsToTry.Add(originalPath);

            // 2. Calea absolută construită din AppDomain + cale relativă
            if (!Path.IsPathRooted(originalPath))
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, originalPath);
                pathsToTry.Add(fullPath);
            }

            // 3. Calea din AppData
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RestaurantManagementApp", "ProductImages", fileName);
            pathsToTry.Add(appDataPath);

            // 4. Directorul Products din aplicație
            string productsPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "Images", "Products", fileName);
            pathsToTry.Add(productsPath);

            // Încearcă fiecare cale
            foreach (string path in pathsToTry)
            {
                try
                {
                    if (File.Exists(path))
                    {
                        System.Diagnostics.Debug.WriteLine($"File exists at: {path}");

                        // Metoda 1: Stream
                        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = stream;
                            bitmap.EndInit();
                            bitmap.Freeze();

                            System.Diagnostics.Debug.WriteLine($"Successfully loaded from: {path}");
                            return bitmap;
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"File does not exist at: {path}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading from {path}: {ex.Message}");
                }
            }

            System.Diagnostics.Debug.WriteLine("Failed to load image from any location");
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static BitmapImage CreateBlankImage(int width, int height)
        {
            var renderTargetBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            var visual = new DrawingVisual();

            using (var context = visual.RenderOpen())
            {
                context.DrawRectangle(
                    Brushes.LightGray,
                    new Pen(Brushes.Gray, 1),
                    new Rect(0, 0, width, height));

                var formattedText = new FormattedText(
                    "No image",
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"),
                    12,
                    Brushes.DarkGray,
                    VisualTreeHelper.GetDpi(visual).PixelsPerDip);

                context.DrawText(
                    formattedText,
                    new Point((width - formattedText.Width) / 2, (height - formattedText.Height) / 2));
            }

            renderTargetBitmap.Render(visual);

            var bitmapImage = new BitmapImage();
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                stream.Position = 0;

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }

            return bitmapImage;
        }
    }
}