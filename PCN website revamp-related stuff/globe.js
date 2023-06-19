// Globe tasks!!! These are run after the entire page loads, so this script is located after the end of the <body> element
globeContainer = document.getElementsByClassName("globeContainer")[0];

//~ Something that will be helpful: For the arcs representing real-time trades, there could be a variable containing the list of arcs, and that variable can be periodically updated (every few seconds) to prune already-finished trades and start arcs for new ones. The Globe.GL website has examples that can be referenced in creating this functionality.
const wxGlobe = Globe()(globeContainer)
    // To prepare the globe view
    .width(width).height(height+60)
    .backgroundColor("rgba(0,0,0,0)")
    .globeImageUrl("Graphics/globetexture.jpg")
    .atmosphereColor("rgb(0,0,0)")
    .pointOfView({lat:0,lng:-75,altitude:2.6})
    
    // To make the flying arcs
    .arcsData([
        { // From Kyoto to Redmond
            startLat: 35.0210700,
            startLng: 135.7538500,
            endLat: 47.673988,
            endLng: -122.121513,
            color: "#ff0000",
            name: "<div style='background-color:rgb(0,0,0);padding:5px;font-family:\"Hiragino Maru Gothic ProN\";border-radius:5px;'>Between Kyoto and Redmond</div>"
        },
        { // From Redmond to Kyoto
            startLat: 47.673988,
            startLng: -122.121513,
            endLat: 35.0210700,
            endLng: 135.7538500,
            color: "#0000ff", // Gradients are being weird, but at least this works!
            name: "<div style='background-color:rgb(0,0,0);padding:5px;font-family:\"Hiragino Maru Gothic ProN\";border-radius:5px;'>Between Kyoto and Redmond</div>"
        },
        { // From New York to Paris
            startLat: 40.730610,
            startLng: -73.93524,
            endLat: 48.8534100,
            endLng: 2.3488000,
            color: "#ffffff",
            name: "<div style='background-color:rgb(0,0,0);padding:5px;font-family:\"Hiragino Maru Gothic ProN\";border-radius:5px;'>Between New York and Paris</div>"
        },
        { // From Paris to New York
            startLat: 48.8534100,
            startLng: 2.3488000,
            endLat: 40.730610,
            endLng: -73.93524,
            color: "#000000",
            name: "<div style='background-color:rgb(0,0,0);padding:5px;font-family:\"Hiragino Maru Gothic ProN\";border-radius:5px;'>Between New York and Paris</div>"
        }
    ])
    .arcStroke(1)
    .arcColor('color')
    .arcDashLength(0.5)
    .arcDashGap(1.5)
    .arcDashAnimateTime(5000)
    .arcAltitude(0.2);

// Auto-rotating looks cool, but it makes it harder to control the globe and examine trades at different angles. There can probably be a toggle for starting or stopping auto-rotate.
//wxGlobe.controls().autoRotate = true;
//wxGlobe.controls().autoRotateSpeed = 1;

// End of globe tasks



// setTimeouts for real-time updates - coming soon!!!
