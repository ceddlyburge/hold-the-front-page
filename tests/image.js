/*
 * ImageResize for Prototype
 * Version 1.0 | Feb 20, 2008
 *
 * Create a new pannable, resizeable image. Image dimensions and x/y offset
 * will be stored in hidden inputs that can then be passed to a form.
 * 
 * Requirements: Prototype, Scriptaculous (Control, Slider)
 * Important Note: Image tag must have width/height specified.
 *
 * Copyright (C) 2008 Nicholas Cook (iamnick@gmail.com)
 * 
 *
 * LICENSE AGREEMENT
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.

 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>
 */
function ImageResize(foregroundelement, element, width, height, imagex, imagey, imagesize, allow_resize, handle, track)
{
    foregroundelement         = $(foregroundelement);
    element         = $(element);
    var parent      = element.getOffsetParent();
    width           = (typeof width         != 'undefined') ? width         : 400;
    height          = (typeof height        != 'undefined') ? height        : 400;
    imageX          = (typeof imagex        != 'undefined') ? $(imagex)     : $('imagex');
    imageY          = (typeof imagey        != 'undefined') ? $(imagey)     : $('imagey');
    imageSize       = (typeof imagesize     != 'undefined') ? $(imagesize)  : $('imagesize');
    handle          = (typeof handle        != 'undefined') ? $(handle)     : $('scale-handle');
    track           = (typeof track         != 'undefined') ? $(slider)     : $('scale-track');
    allow_resize    = (typeof allow_resize  != 'undefined') ? allow_resize : true;
    
    var originSize, initialScale;
    
    var mouseDownX, mouseDownY;
    var newOffsetX, newOffsetY;
    
    // initializer
    function init()
    {
        // events on frame
        parent.observe('mousedown', panMouseDown);
        
        // remove mouse events on image
        element.onmousedown = element.onmousemove = function(){ return false; }; // remove mousedown from img
        foregroundelement.onmousedown = foregroundelement.onmousemove = function(){ return false; }; 
        
        // set size of container box
        parent.setStyle({
            width:  width + 'px',
            height: height + 'px'
        });
        
        // stop processing if image resizing is turned off
        if(!allow_resize) return false;
        
        originSize          = element.getDimensions();
        var image_min       = Math.min(originSize.width, originSize.height);
        var box_max         = Math.max(width, height);
        var down_ratio      = box_max / image_min;
        var up_ratio        = image_min / box_max;
        initialScale        = { width: Math.ceil(originSize.width * down_ratio),
                                height: Math.ceil(originSize.height * down_ratio) };

        // resize the element
        element.setStyle({
            width:  initialScale.width + 'px',
            height: initialScale.height + 'px'
        });
        
        // update hidden input
        imageSize.writeAttribute('value', initialScale.width + 'x' + initialScale.height);
        
        // create the scale slider
        var slider = new Control.Slider(handle, track, {
            range: $R(1, up_ratio),
            sliderValue: 1,
            onChange: scaleSlideChange,
            onSlide: scaleSlideMove
        });
    }

    // Mouse Event: capture the mouse down and start observing the move
    function panMouseDown(event)
    {
        mouseDownX = event.pointerX();
        mouseDownY = event.pointerY();

        // observe mouse moving
        $(document.body).observe('mousemove', panMouseMove);
        $(document.body).observe('mouseup', panMouseUp);
    }
    
    // Mouse Event: remove mouse move and adjust hidden input values
    function panMouseUp(event)
    {
        var dimensions = element.getDimensions();
        imageX.writeAttribute('value', newOffsetX);
        imageY.writeAttribute('value', newOffsetY);
        
        // stop events
        $(document.body).stopObserving('mousemove', panMouseMove);
        $(document.body).stopObserving('mouseup', panMouseUp);
    }
    
    // Mouse Event: observe the mouse move and adjust the position
    function panMouseMove(event)
    {
        var dimensions  = element.getDimensions();
        var x           = event.pointerX() - mouseDownX;
        var y           = event.pointerY() - mouseDownY;

        var offsetX     = parseInt(imageX.readAttribute('value'), 10) + x;
        var offsetY     = parseInt(imageY.readAttribute('value'), 10) + y;
        
        offsetX = newOffsetX = (offsetX >= 0) ? 0:
                            (offsetX + dimensions.width < width) ? (-1 * dimensions.width + width) : offsetX;
        offsetY = newOffsetY = (offsetY >= 0) ? 0:
                            (offsetY + dimensions.height < height) ? (-1 * dimensions.height + height) : offsetY;

        element.setStyle({
            left: offsetX + 'px',
            top:  offsetY + 'px'
        });
    }
    
    // Slider Event: update hidden inputs on slider change
    function scaleSlideChange(value)
    {
        scaleSlideMove(value);
        
        var dimensions = element.getDimensions();
        imageSize.writeAttribute('value', dimensions.width + 'x' + dimensions.height);
        imageX.writeAttribute('value', parseInt(element.getStyle('left'), 10));
        imageY.writeAttribute('value', parseInt(element.getStyle('top'), 10));
    }
    
    // Slider Event: scale image based on handle position
    function scaleSlideMove(value)
    {
        var dimensions      = element.getDimensions();
        var newWidth        = Math.ceil(initialScale.width * value);
        var newHeight       = Math.ceil(initialScale.height * value);
        
        element.setStyle({
            width:  newWidth + 'px',
            height: newHeight + 'px'
        });
        
        // update positioning if resized from right/bottom edge
        var left            = parseInt(element.getStyle('left'), 10);
        var top             = parseInt(element.getStyle('top'), 10);
        var left_pixels     = newWidth + left;
        var top_pixels      = newHeight + top;
        
        if(left_pixels < width)
            element.style.left = left + width - left_pixels + 'px';
        
        if(top_pixels < height)
            element.style.top = top + width - top_pixels + 'px';
    }
    
    init();
}