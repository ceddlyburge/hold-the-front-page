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
function ImageResize(foregroundelement, element, width, height, /*cedd*/inupratio, imagex, imagey, imagesize, allow_resize, handle, track)
{
    foregroundelement         = $(foregroundelement); // cedd
    var foregroundparent      = foregroundelement.getOffsetParent(); //cedd
    inupratio       = (typeof inupratio         != 'undefined') ?inupratio         : 2; // cedd   
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
    
    var originSize, initialScale, downScale;
    var originmidx, originmidy;
    
    var mouseDownX, mouseDownY;
    var newOffsetX, newOffsetY;
    
    // initializer
    function init()
    {
        // events on frame
        parent.observe('mousedown', panMouseDown);
        foregroundparent.observe('mousedown', panMouseDown); //cedd
        
        // remove mouse events on images
        element.onmousedown = element.onmousemove = function(){ return false; }; 
        foregroundelement.onmousedown = foregroundelement.onmousemove = function(){ return false; }; //cedd
        
        // set size of container box
        parent.setStyle({
            width:  width + 'px',
            height: height + 'px'
        });
        
        // stop processing if image resizing is turned off
        if(!allow_resize) return false;
        
        debugger;
        originSize          = element.getDimensions();
        var image_min       = Math.min(originSize.width, originSize.height);
        var box_max         = Math.max(width, height);
        //cedd replaced var down_ratio      = box_max / image_min;
        var down_ratio      = Math.max(width / originSize.width, height / originSize.height);
        var up_ratio        = inupratio; //cedd replaced following (2 * image_min) / box_max;
        var initial_ratio   = imageSize.value;

        downScale        = { width: Math.ceil(originSize.width * down_ratio),
                             height: Math.ceil(originSize.height * down_ratio) };

        initialScale        = { width: Math.ceil(originSize.width * initial_ratio),
                                height: Math.ceil(originSize.height * initial_ratio) };

        // resize the element
        element.setStyle({
            width:  initialScale.width + 'px',
            height: initialScale.height + 'px'
        });
        
        // update hidden input
        imageSize.writeAttribute('value', initial_ratio);
        
        // create the scale slider
        var slider = new Control.Slider(handle, track, {
            range: $R(down_ratio, up_ratio),
            sliderValue: initial_ratio,
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
        
        // set the desired midpoint of the non zoomed image after this pan
        var scale   = imageSize.readAttribute('value'); //cedd
        originmidx = (-offsetX + (width / 2)) / scale; //cedd
        originmidy = (-offsetY + (height / 2)) / scale; //cedd
    }
    
    // Slider Event: update hidden inputs on slider change
    function scaleSlideChange(value)
    {
        scaleSlideMove(value);
        
        var dimensions = element.getDimensions();
        imageSize.writeAttribute('value', value);
        imageX.writeAttribute('value', parseInt(element.getStyle('left'), 10));
        imageY.writeAttribute('value', parseInt(element.getStyle('top'), 10));
    }
    
    // Slider Event: scale image based on handle position
    function scaleSlideMove(value)
    {
    
        var dimensions      = element.getDimensions();
        var newWidth        = Math.ceil(originSize.width * value);
        var newHeight       = Math.ceil(originSize.height * value);
        
        element.setStyle({
            width:  newWidth + 'px',
            height: newHeight + 'px'
        });
        
        // cedd. update positioning when zooming so that the part of the picture in view stays in view
        //debugger;
        var left       = parseInt(element.getStyle('left'), 10);
        var top        = parseInt(element.getStyle('top'), 10);
        var oldscale   = imageSize.readAttribute('value'); //cedd
        
        // set the desired midpoint of the non zoomed image if not already done
        if (originmidx == null) originmidx = (-left + (width / 2)) / oldscale; 
        if (originmidy == null) originmidy = (-top + (height / 2)) / oldscale; 
        
        // keep this midpoint in the middle after the zoom
        left = -((originmidx * value) - (width / 2));
        top = -((originmidy * value) - (height / 2));
        //imageSize.writeAttribute('value', value); 
        
        // check image is taking up all of the parent box
        if ((-left + width) > newWidth) left = width -newWidth;
        if ((-top + height) > newHeight) top = height -newHeight;
        if (left > 0) left = 0;
        if (top > 0) top = 0;

        // update image position
        element.style.left = left + 'px';
        element.style.top = top + 'px';
    }
    
    init();
}