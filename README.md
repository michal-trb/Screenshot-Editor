# screenerWpf

## Overview

screenerWpf is a Windows Presentation Foundation (WPF) application designed for screen capturing, video recording, and basic image editing. It includes a variety of tools to help users capture specific areas of the screen, record videos, and edit images with intuitive drawing and annotation features.

Design:

![screenshot-editor](https://github.com/user-attachments/assets/49d6195f-045d-4ba4-a89c-d07b06266630)

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
    git clone https://github.com/michal-trb/Screenshot-Editor.git
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

The **screenerWpf** project is organized into several core components, each responsible for distinct functionality. Below, we'll describe each of these components in more detail.

### 1. Models
This folder contains the core data structures that define and represent various entities in the application, helping to manage the data needed for different functionalities.

- **Drawable Elements** (`DrawableText`, `DrawableArrow`, `DrawableSpeechBubble`, etc.):
  - Represents visual elements that can be drawn on a canvas. 
  - Each drawable element is a subclass of `DrawableElement` and provides custom behaviors like rendering (`Draw()` method), selection (`HitTest()` method), and boundary calculations (`GetBounds()`).
  - Examples:
    - **`DrawableText`**: Allows text to be added to screenshots with configurable typeface and color.
    - **`DrawableArrow`**: Enables adding arrows to mark specific parts of an image.
    - **`DrawableSpeechBubble`**: Adds speech bubbles for adding comments visually.

- **LastScreenshot & LastVideo**:
  - Represents recently captured media files to be displayed in the main window for quick access.
  - **`LastScreenshot`**:
    - Stores metadata about the screenshot such as file path, file name, and a thumbnail for easy preview.
  - **`LastVideo`**:
    - Similar to `LastScreenshot`, but specific to video files. It also generates thumbnails using `MediaToolkit` for a quick preview.

- **ScreenRecorder**:
  - A wrapper that uses **ScreenRecorderLib** for video recording.
  - Handles starting, stopping, and configuration of screen recordings.
  - It manages recording areas or the entire screen, including setting up specific settings for recording.

### 2. Services
This folder contains classes responsible for handling specific, reusable logic in the application. Services are used to manage actions like capturing images, recording screens, managing windows, and performing OCR.

- **Capture Services**:
  - Responsible for the logic around capturing images and video.
  - Examples:
    - **`ScreenCaptureService`**:
      - Acts as the main entry point for capturing screenshots and videos.
      - Supports full screen, area selection, and window-specific captures.
    - **`AreaScreenshot` & `FullScreenshot`**:
      - **`AreaScreenshot`**: Captures a specific region of the screen, defined by a rectangle.
      - **`FullScreenshot`**: Captures the entire screen, adjusting for DPI settings.
    - **`WindowScreenshot`**:
      - Captures specific application windows using `PrintWindow` API and GDI methods.

- **WindowService**:
  - Manages interaction with various UI windows, such as dialogs or overlays.
  - Examples:
    - **`AreaSelector`**: Provides a translucent overlay for selecting a specific area of the screen to capture.
    - **`VideoPlayerWindow`**: Handles video playback for recorded videos.
    - **`StopRecordingWindow`**: Displays a control for stopping a recording.

### 3. Views
This folder contains the XAML UI definitions for the different windows and controls of the application.

- **Windows and Controls**:
  - Each window represents a different part of the user interface.
  - Examples:
    - **`OptionsWindow`**:
      - Allows users to configure application settings, such as defining paths for screenshots, videos, and themes.
    - **`ResizeDialog`**:
      - Provides a dialog to change the dimensions of an image, with options to specify height, width, or resize by percentage.
    - **`OverlayWindow` & `ControlWindow`**:
      - **`OverlayWindow`**: Used during area selection to show a translucent rectangle over the screen to guide the user in selecting the desired area.
      - **`ControlWindow`**: A small utility window that allows the user to stop a recording or complete an area selection.

### 4. ViewModels

The **ViewModels** in the `screenerWpf` project follow the **MVVM (Model-View-ViewModel)** pattern, which helps in decoupling the UI from the underlying business logic. These **ViewModels** provide data and commands for the views, and they manage the overall state of the application.

#### OptionsViewModel
- **`OptionsViewModel`** handles the configuration settings of the application. 
- It manages paths for screenshots, recordings.
- Properties like `ScreenshotsSavePath`, `RecordsSavePath` allow users to adjust where the captured media is saved or synced.

#### MainViewModel
- **`MainViewModel`** is the central point for managing the core actions available in the application. 
- It ties together services and commands for capturing screenshots, recording videos, and editing images.
  
  Key functionalities:
  - **Capturing Screenshots and Videos**:
    - Commands like `CaptureFullCommand`, `CaptureAreaCommand`, and `CaptureWindowCommand` are provided for taking screenshots.
    - Video recording commands such as `RecordVideoCommand` and `RecordAreaVideoCommand` are implemented for different recording scenarios.
  - **Data Management**:
    - Keeps track of recently captured screenshots and videos by maintaining collections like `LastScreenshots` and `LastVideos`.
    - Provides properties to bind UI elements for easy access to the most recent captures.
  - **Command Management**:
    - Includes commands for different actions, like opening the options window, stopping recordings, and more.
    - Integrates services such as `ScreenCaptureService` to ensure each command is properly linked with the underlying functionality.

  The `MainViewModel` plays an essential role in facilitating communication between the core services and the UI, ensuring that every user action is connected to the appropriate backend process.

#### ImageEditorViewModel
- **`ImageEditorViewModel`** is the primary ViewModel for managing the state and behavior of the **Image Editor**.
- It is responsible for commands and data binding used when editing images, such as drawing shapes, adding text, and modifying visual elements.

  Key functionalities:
  - **Element Management**:
    - Provides collections of drawable elements (`Elements`) that represent different items users can add to an image (like text, arrows, and shapes).
    - Manages the active selection and controls properties like color, thickness, transparency, and font attributes for those selected elements.
  - **Drawing and Editing Commands**:
    - Commands like `DrawArrowCommand`, `DrawRectCommand`, `AddTextCommand`, `BlurCommand`, and `BrushCommand` provide the user with tools to modify images.
    - Supports undo/redo actions through commands like `UndoCommand` and `RedoCommand`, making it easier for users to revert or reapply changes.
  - **Binding for Properties**:
    - Properties like `SelectedColor`, `SelectedThickness`, `SelectedFontFamily`, `SelectedFontSize`, etc., are bound to controls in the image editor UI.
    - This allows for a dynamic and interactive editing experience, where changing a property immediately affects the selected element on the canvas.
  - **Canvas Interaction**:
    - `ImageEditorViewModel` interacts with the **`DrawableCanvas`** control, providing the data for items to draw and controlling how user interactions (like moving or resizing elements) are processed.
    - Commands such as `ResizeCommand` allow users to resize the current image based on the dimensions specified, while properties such as `Elements` keep track of all drawable elements on the canvas.

### 5. Controls
- **Custom Controls** (`ImageEditorControl`, `DrawableCanvas`):
  - These are custom user controls that provide specific features like editing images, drawing on the canvas, and integrating multiple drawables.
  - **`ImageEditorControl`**:
    - This control hosts the drawable canvas, toolbars, and controls that allow a user to interact with and edit an image.
    - It includes commands for various editing tools such as adding arrows, text, or shapes.
  - **`DrawableCanvas`**:
    - The actual canvas on which users can draw. The `DrawableCanvas` hosts different drawable elements and provides interaction logic, such as moving elements, resizing, etc.

### 6. Resources
- **Styles and Resources**:
  - This folder contains XAML files that define the application's appearance, including styles, colors, brushes, and icons.
  - **LightStyle.xaml**:
    - Defines the color scheme for the light theme.
    - Provides definitions for colors like `Background900`, `TitleBarMain900`, etc., and styles for UI elements (`Button`, `TextBox`, etc.).

### 7. Commands and Actions
Commands handle the logic that responds to user inputs, such as clicks or keyboard shortcuts.

- **Undoable Actions**:
  - **`AddElementAction`** and **`RemoveElementAction`**:
    - Implements `IUndoableAction` interface, allowing actions to be undone or redone by adding or removing drawable elements.
    - This functionality is crucial for providing a good user experience when making changes to images.

## Summary
The **File Structure** of `screenerWpf` is carefully designed to keep various responsibilities isolated:

- **Models** define the data structures and behaviors of visual elements.
- **Services** provide reusable functionality for capturing images, managing windows.
- **Views** define the user interface, and **ViewModels** provide the data-binding and application logic for these views.
- **Controls** add user interface elements like the image editor and drawable canvas.
- **Resources** provide styling and theme configurations to keep the UI consistent and customizable.
- **Commands and Actions** enable user interaction, ensuring actions can be executed, undone, or repeated intuitively.

This modular organization helps in maintaining a clean, scalable, and easy-to-understand codebase that is both flexible and extendable.


## License

This project is licensed under the MIT License.

## Credits

- **[ScreenRecorderLib](https://github.com/sskodje/ScreenRecorderLib)** for enabling screen recording functionality.
- **[MediaToolkit](https://github.com/AydinAdn/MediaToolkit)** for video thumbnail generation.
- **[ColorPicker](https://github.com/PixiEditor/ColorPicker)** for easy to use color picker 

Author Michał Trybulec
