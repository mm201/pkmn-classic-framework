// Get the viewport width and height, window height, and the coordinates of the window (will be important for later functions!)
var width, height, windowheight, fromleft, fromtop; // They need to be global



function updateWHVars () {
    //~ Viewport width and height
    if(window.innerWidth != undefined && window.innerHeight != undefined) {
        width = window.innerWidth;
        height = window.innerHeight;
    } else {
        width = document.documentElement.clientWidth;
        height = document.documentElement.clientHeight;
        // Hopefully this failsafe works? I'll have to double-check with IE6-8 in an emulator...
    }
    //~ Window width and height (no failsafe for IE yet)
    windowheight = window.outerHeight;
    //~ Coordinates of the window
    if(window.screenX != undefined && window.screenY != undefined) {
        fromleft = window.screenX;
        fromtop = window.screenY;
    } else {
        fromleft = window.screenLeft;
        fromtop = window.screenTop;
        // Hopefully this failsafe works? I'll have to double-check with IE on a different machine...
    }
}



function infoPopup () {
    updateWHVars();

    window.open("credits/home.html","credits","width=240px,height=480px,left="+(fromleft+width-240)+"px,top="+(fromtop+windowheight-height+38)+"px,toolbar=no");
}



function tabChanges (gen4width, gen5width) {
    // Tab contents for each generation
    gen4contents = "<ul> <li>GTS</li> <li>Battle Videos</li> <li>Dressup</li> <li>Box Uploads</li> <li>Wi-Fi Battle Tower</li> <li>Trainer Rankings [X]</li> <li>Wi-Fi Plaza [X]</li> </ul>";
    gen5contents = "<ul> <li>GTS</li> <li>Battle Videos</li> <li>Musical Photos</li> <li>Wi-Fi Battle Subway</li> <li>Trainer Rankings [X]</li> <li>Game Sync [X]</li> </ul>";

    // Define Gen 4 and Gen 5 tabs, and "features" div
    gen4tab = document.getElementsByClassName("gen4")[0];
    gen5tab = document.getElementsByClassName("gen5")[0];
    featureslist = document.getElementsByClassName("features")[0];
    
    
    
    // Set styles
    gen4tab.style.width = gen4width + "%";
    gen5tab.style.width = gen5width + "%";
    
    // Change the contents of the "features" box appropriately
    if (gen4width > gen5width) {
        featureslist.innerHTML = gen4contents;
    } else {
        featureslist.innerHTML = gen5contents;;
    }
}



function adjustSize () {
    updateWHVars();

    // Set up the test to check for CSS calc() support
    // Source: https://stackoverflow.com/questions/14125415/how-can-i-check-if-css-calc-is-available-using-javascript
    el = document.createElement('div');
    el.style.cssText = "width: calc(10px)";
    if (!el.style.length) {
        // Set CSS of important elements that rely on dynamic width and height
        //~ Body (in case you need to do "height: 100%;" or something like that)
        document.body.style.width = width + "px";
        document.body.style.height = height + "px";
        
        //~ Left and right sidebars
        leftSidebar = document.getElementsByClassName("left")[0];
        rightSidebar = document.getElementsByClassName("right")[0];
        
        leftSidebar.style.height = (height - 155) + "px";
        rightSidebar.style.height = (height - 155) + "px";
        
        //~ "About" section
        aboutSection = document.getElementsByClassName("about")[0];
        //aboutContent = aboutSection.getElementsByClassName("content")[0];
        
        aboutSection.style.height = (height - 316) + "px";
        //aboutContent.style.height = (height - 352) + "px"; //<-- failed attempt to mak this look right in DSi Browser
        
        //~ Trades ticker (todo)
        
        //~ globeContainer (todo)
        
    }
    
    // Below are the ones that need to be resized regardless

    //~ Twitter embed (todo: the below code doesn't work — Twitter embed is being quite finicky! This section will be commented out for the time being.)
    //~ Oh! Maybe...I could make something where the Twitter embed gets removed, then gets re-added with a different height value?? Or would that break some sort of terms-of-use?...
    //twitterEmbed = document.getElementsByClassName("twitter-timeline")[0];
    
    //twitterEmbed.style.height = "300px";
    
    
    //~ Resize the Globe.GL instance
    if (wxGlobe != undefined) {
        wxGlobe.width(width).height(height+60);
    }
}
