# screenerWpf

## Overview

screenerWpf is a Windows Presentation Foundation (WPF) application designed for screen capturing, video recording, and basic image editing. It includes a variety of tools to help users capture specific areas of the screen, record videos, and edit images with intuitive drawing and annotation features.

## Features

### 1. Screen Capture and Video Recording
- Capture the entire screen, a specific window, or a custom rectangular area.
- Record videos of the entire screen or selected areas.
- Save screenshots and recordings to a designated folder.
- Manage captured media through easy access to recent screenshots and videos.

### 2. Image Editing
- Draw annotations like arrows, rectangles, and speech bubbles on captured screenshots.
- Add text or blur parts of an image for privacy.
- Use a brush tool to freehand draw.
- Resize images and adjust transparency settings.

### 4. Customization
- Configurable options to define paths for saving screenshots and videos.

## Dependencies

### External Libraries and Tools
- **WPF** for UI elements and interactions.
- **MediaToolkit** for capturing video thumbnails.
- **ScreenRecorderLib** for recording video of the screen.
- **ColorPicker** for color selection when annotating images.
  
## Installation

1. **Clone the Repository:**
    ```bash
    git clone https://github.com/yourusername/screenerWpf.git
    ```
2. **Install Dependencies:**
    Ensure you have the necessary NuGet packages installed. You can use the `Package Manager Console` in Visual Studio to install them:
    ```bash
    Install-Package ScreenRecorderLib
    Install-Package Tesseract
    Install-Package MediaToolkit
    ```

3. **Build the Project:**
    Open the solution in Visual Studio, restore any missing dependencies, and build the project.

## Usage

### Screen Capture
1. Use the main window buttons to capture the full screen, a selected window, or a custom area.
2. Recent screenshots will appear in the recent screenshots section of the application.

### Video Recording
1. Start a screen recording by selecting the recording button.
2. Record the entire screen or a selected area.
3. Stop recording to save the video automatically to the configured directory.

### Editing Tools
1. Open the image editor from the application or select an image to edit.
2. Use tools like **Arrow**, **Text**, **Rectangle**, **Blur**, **Speech Bubble**, and **Brush**.
3. Adjust properties like color, thickness, font family, size, and transparency for the selected tools.

## Configuration

### Settings in the Application
- **Screenshots Save Path**: Define the directory for saving screenshots.
- **Records Save Path**: Define the directory for video recordings.
- **Theme Configuration**: Toggle between Light and Dark modes in the Options window.

### Settings through `OptionsViewModel`
Settings are managed via `OptionsViewModel` to save paths and app preferences. You can configure:
- `ScreenshotsSavePath`
- `ScreenshotsLibraryPath`
- `RecordsSavePath`

These values are loaded and saved in the user settings (`Properties.Settings.Default`).

## File Structure

### Core Components
- **Models**
  - **Drawable Elements**: Provides different drawable entities like `DrawableText`, `DrawableSpeechBubble`, etc.
  - **LastScreenshot & LastVideo**: Models to represent recent screenshots and videos.
  - **ScreenRecorder**: A wrapper for handling screen recording logic.
  
- **Services**
  - **Capture Services**: Handles screen capture and recording logic (`ScreenCaptureService`, `AreaScreenshot`, `FullScreenshot`, etc.).
  - **WindowService**: Manages windows like area selection and video player.
  - **TextRecognitionHandler**: Handles text recognition from images.

- **Views**
  - **Windows and Controls**: XAML UI views like `OptionsWindow`, `VideoPlayerWindow`, `ResizeDialog`, and `OverlayWindow`.

- **ViewModels**
  - **OptionsViewModel**: Provides data binding for application settings.

## Key Classes

- **ScreenCaptureService**: 
  Handles capturing screenshots of full screen, a selected area, or specific windows. It also manages screen recording features using `ScreenRecorder`.

- **TextRecognitionHandler**:
  Handles OCR functionality to extract text from captured images using Tesseract.

- **Drawable Elements**:
  Implements various drawable components such as text, shapes, and speech bubbles to facilitate user interactions within the image editor.

- **Overlay Window & Area Selection**:
  These components allow the user to visually select parts of the screen, highlighting the areas to be captured.

## License

This project is licensed under the MIT License. See the LICENSE file for details.

## Credits

- **[ScreenRecorderLib](https://github.com/sskodje/ScreenRecorderLib)** for enabling screen recording functionality.
- **[Tesseract OCR](https://github.com/tesseract-ocr/tesseract)** for the text recognition feature.
- **[MediaToolkit](https://github.com/AydinAdn/MediaToolkit)** for video thumbnail generation.

Author Michał Trybulec
