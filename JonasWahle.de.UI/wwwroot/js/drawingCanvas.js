let dotNetReference = null;

window.setupFullscreenListener = (dotNetRef) => {
    dotNetReference = dotNetRef;

    // Listen for fullscreen change events
    document.addEventListener('fullscreenchange', handleFullscreenChange);
    document.addEventListener('webkitfullscreenchange', handleFullscreenChange);
    document.addEventListener('mozfullscreenchange', handleFullscreenChange);
    document.addEventListener('MSFullscreenChange', handleFullscreenChange);
    
    // Listen for orientation changes on mobile
    window.addEventListener('orientationchange', handleOrientationChange);
    window.addEventListener('resize', handleResize);
};

function handleFullscreenChange() {
    const isFullscreen = !!(document.fullscreenElement ||
        document.webkitFullscreenElement ||
        document.mozFullScreenElement ||
        document.msFullscreenElement);

    if (dotNetReference) {
        dotNetReference.invokeMethodAsync('OnFullscreenChange', isFullscreen);
    }
}

function handleOrientationChange() {
    // Delay to allow orientation change to complete
    setTimeout(() => {
        if (dotNetReference) {
            dotNetReference.invokeMethodAsync('OnOrientationChange');
        }
    }, 100);
}

function handleResize() {
    if (dotNetReference) {
        dotNetReference.invokeMethodAsync('OnResize');
    }
}

window.enterFullscreen = (element) => {
    if (element.requestFullscreen) {
        element.requestFullscreen();
    } else if (element.webkitRequestFullscreen) {
        element.webkitRequestFullscreen();
    } else if (element.mozRequestFullScreen) {
        element.mozRequestFullScreen();
    } else if (element.msRequestFullscreen) {
        element.msRequestFullscreen();
    }
};

window.exitFullscreen = () => {
    if (document.exitFullscreen) {
        document.exitFullscreen();
    } else if (document.webkitExitFullscreen) {
        document.webkitExitFullscreen();
    } else if (document.mozCancelFullScreen) {
        document.mozCancelFullScreen();
    } else if (document.msExitFullscreen) {
        document.msExitFullscreen();
    }
};

// Helper function to get adjusted coordinates for fullscreen
function getAdjustedCoordinates(canvas, x, y) {
    const rect = canvas.getBoundingClientRect();
    
    // Check if we're in fullscreen mode
    const isFullscreen = !!(document.fullscreenElement ||
        document.webkitFullscreenElement ||
        document.mozFullScreenElement ||
        document.msFullscreenElement);
    
    if (isFullscreen) {
        // In fullscreen, we need to account for the padding and container positioning
        const container = canvas.closest('.canvas-container');
        const containerRect = container.getBoundingClientRect();
        
        // Calculate the actual canvas position within the fullscreen container
        const canvasOffsetX = rect.left - containerRect.left;
        const canvasOffsetY = rect.top - containerRect.top;
        
        // Adjust coordinates relative to the actual canvas position
        return {
            x: x,
            y: y,
            scaleX: canvas.width / rect.width,
            scaleY: canvas.height / rect.height
        };
    } else {
        // Normal mode - use original calculation
        return {
            x: x,
            y: y,
            scaleX: canvas.width / rect.width,
            scaleY: canvas.height / rect.height
        };
    }
}

window.initializeCanvas = (canvas) => {
    const ctx = canvas.getContext('2d'); // canvas.getContext('2d', { willReadFrequently: true });

    // Set white background
    ctx.fillStyle = 'white';
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    canvas.ctx = ctx;
    canvas.isDrawing = false;
    canvas.lastX = 0;
    canvas.lastY = 0;

    // Initialize preview settings
    canvas.brushColor = '#000000';
    canvas.brushSize = 5;
    canvas.isErasing = false;
    canvas.showPreview = false;

    // Create preview overlay canvas
    const previewCanvas = document.createElement('canvas');
    previewCanvas.style.position = 'absolute';
    previewCanvas.style.top = '0';
    previewCanvas.style.left = '0';
    previewCanvas.style.pointerEvents = 'none';
    previewCanvas.style.zIndex = '10';
    previewCanvas.width = canvas.width;
    previewCanvas.height = canvas.height;
    previewCanvas.style.width = '100%';
    previewCanvas.style.height = '100%';

    canvas.previewCanvas = previewCanvas;
    canvas.previewCtx = previewCanvas.getContext('2d');

    // Insert preview canvas after main canvas
    canvas.parentNode.appendChild(previewCanvas);

    console.log('Canvas initialized with brush preview');
};

// New function to resize canvas for fullscreen
window.resizeCanvasForFullscreen = (canvas) => {
    const isFullscreen = !!(document.fullscreenElement ||
        document.webkitFullscreenElement ||
        document.mozFullScreenElement ||
        document.msFullscreenElement);
    
    if (isFullscreen) {
        // Store original canvas data
        const originalData = canvas.ctx.getImageData(0, 0, canvas.width, canvas.height);
        const originalWidth = canvas.width;
        const originalHeight = canvas.height;
        
        // Get screen dimensions
        const screenWidth = screen.width * window.devicePixelRatio;
        const screenHeight = screen.height * window.devicePixelRatio;
        
        // Use screen dimensions for fullscreen canvas
        canvas.width = screenWidth;
        canvas.height = screenHeight;
        
        // Update preview canvas dimensions
        if (canvas.previewCanvas) {
            canvas.previewCanvas.width = screenWidth;
            canvas.previewCanvas.height = screenHeight;
        }
        
        // Restore canvas context properties
        const ctx = canvas.ctx;
        ctx.fillStyle = 'white';
        ctx.fillRect(0, 0, canvas.width, canvas.height);
        
        // Scale and redraw the original content
        ctx.drawImage(
            createImageFromImageData(originalData, originalWidth, originalHeight),
            0, 0, originalWidth, originalHeight,
            0, 0, canvas.width, canvas.height
        );
        
        console.log(`Canvas resized for fullscreen: ${canvas.width}x${canvas.height}`);
    } else {
        // Restore original dimensions when exiting fullscreen
        restoreOriginalCanvasSize(canvas);
    }
};

// Helper function to create image from ImageData
function createImageFromImageData(imageData, width, height) {
    const tempCanvas = document.createElement('canvas');
    tempCanvas.width = width;
    tempCanvas.height = height;
    const tempCtx = tempCanvas.getContext('2d');
    tempCtx.putImageData(imageData, 0, 0);
    return tempCanvas;
}

// Restore original canvas size
window.restoreOriginalCanvasSize = (canvas) => {
    // Store current canvas data
    const currentData = canvas.ctx.getImageData(0, 0, canvas.width, canvas.height);
    const currentWidth = canvas.width;
    const currentHeight = canvas.height;
    
    // Restore original dimensions
    canvas.width = 1920;
    canvas.height = 1080;
    
    // Update preview canvas dimensions
    if (canvas.previewCanvas) {
        canvas.previewCanvas.width = 1920;
        canvas.previewCanvas.height = 1080;
    }
    
    // Restore canvas context properties
    const ctx = canvas.ctx;
    ctx.fillStyle = 'white';
    ctx.fillRect(0, 0, canvas.width, canvas.height);
    
    // Scale and redraw the fullscreen content back to original size
    ctx.drawImage(
        createImageFromImageData(currentData, currentWidth, currentHeight),
        0, 0, currentWidth, currentHeight,
        0, 0, canvas.width, canvas.height
    );
    
    console.log(`Canvas restored to original size: ${canvas.width}x${canvas.height}`);
};

window.updateBrushSettings = (canvas, color, size, isErasing) => {
    canvas.brushColor = color;
    canvas.brushSize = size;
    canvas.isErasing = isErasing;
};

window.showPreview = (canvas) => {
    canvas.showPreview = true;
};

window.hidePreview = (canvas) => {
    canvas.showPreview = false;
    // Clear preview canvas
    if (canvas.previewCtx) {
        canvas.previewCtx.clearRect(0, 0, canvas.previewCanvas.width, canvas.previewCanvas.height);
    }
};

window.updatePreview = (canvas, x, y) => {
    if (!canvas.showPreview || !canvas.previewCtx) return;

    const previewCtx = canvas.previewCtx;
    const coords = getAdjustedCoordinates(canvas, x, y);

    // Scale the coordinates for preview
    const scaledX = coords.x * coords.scaleX;
    const scaledY = coords.y * coords.scaleY;
    const scaledSize = canvas.brushSize * coords.scaleX;

    // Clear previous preview
    previewCtx.clearRect(0, 0, canvas.previewCanvas.width, canvas.previewCanvas.height);

    // Draw preview circle
    previewCtx.beginPath();
    previewCtx.arc(scaledX, scaledY, scaledSize / 2, 0, 2 * Math.PI);

    if (canvas.isErasing) {
        // Show eraser preview with dashed outline
        previewCtx.strokeStyle = '#ff0000';
        previewCtx.setLineDash([5, 5]);
        previewCtx.lineWidth = 2;
        previewCtx.stroke();
        previewCtx.setLineDash([]);
    } else {
        // Show brush preview with semi-transparent fill and solid outline
        previewCtx.fillStyle = canvas.brushColor + '40'; // Add transparency
        previewCtx.fill();
        previewCtx.strokeStyle = canvas.brushColor;
        previewCtx.lineWidth = 1;
        previewCtx.stroke();
    }
};

window.startDrawing = (canvas, x, y, color, size, isErasing) => {
    const ctx = canvas.ctx;
    const coords = getAdjustedCoordinates(canvas, x, y);

    // Scale the coordinates
    const scaledX = coords.x * coords.scaleX;
    const scaledY = coords.y * coords.scaleY;

    canvas.isDrawing = true;
    canvas.lastX = scaledX;
    canvas.lastY = scaledY;

    // Hide preview while drawing
    if (canvas.previewCtx) {
        canvas.previewCtx.clearRect(0, 0, canvas.previewCanvas.width, canvas.previewCanvas.height);
    }

    ctx.globalCompositeOperation = isErasing ? 'destination-out' : 'source-over';
    ctx.strokeStyle = color;
    ctx.fillStyle = color;
    ctx.lineWidth = size * coords.scaleX; // Scale brush size too
    ctx.lineCap = 'round';
    ctx.lineJoin = 'round';

    // Draw a dot at the start position
    ctx.beginPath();
    ctx.arc(scaledX, scaledY, (size * coords.scaleX) / 2, 0, 2 * Math.PI);
    ctx.fill();
};

window.draw = (canvas, x, y) => {
    if (!canvas.isDrawing || !canvas.ctx) return;

    const ctx = canvas.ctx;
    const coords = getAdjustedCoordinates(canvas, x, y);

    // Scale the coordinates
    const scaledX = coords.x * coords.scaleX;
    const scaledY = coords.y * coords.scaleY;

    ctx.beginPath();
    ctx.moveTo(canvas.lastX, canvas.lastY);
    ctx.lineTo(scaledX, scaledY);
    ctx.stroke();

    canvas.lastX = scaledX;
    canvas.lastY = scaledY;
};

window.clearCanvas = (canvas) => {
    const ctx = canvas.ctx;
    if (!ctx) return;

    ctx.fillStyle = 'white';
    ctx.fillRect(0, 0, canvas.width, canvas.height);
    console.log('Canvas cleared');
};

window.downloadCanvas = (canvas, filename) => {
    try {
        console.log('Download attempt - Canvas dimensions:', canvas.width, 'x', canvas.height);

        const url = canvas.toDataURL('image/png', 1.0);

        if (!url || url === 'data:,') {
            console.error('Failed to generate valid image data');
            return;
        }

        const a = document.createElement('a');
        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);

        console.log('Canvas downloaded successfully');
    } catch (error) {
        console.error('Error downloading canvas:', error);
    }
};

window.getCanvasBoundingRect = (canvas) => {
    const rect = canvas.getBoundingClientRect();
    return {
        left: rect.left,
        top: rect.top,
        right: rect.right,
        bottom: rect.bottom,
        width: rect.width,
        height: rect.height
    };
};