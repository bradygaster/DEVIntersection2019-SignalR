const connection = new signalR.HubConnectionBuilder()
    .withUrl('/hubs/rotation')
    .configureLogging(signalR.LogLevel.Information)
    .build();

function handleOrientation(event) {
    var alpha = event.alpha;
    var beta = event.beta;
    var gamma = event.gamma;

    document.getElementById("alpha").innerHTML = event.alpha;
    document.getElementById("beta").innerHTML = event.beta;
    document.getElementById("gamma").innerHTML = event.gamma;

    if (connection) {
        connection.invoke('rotate',
            event.beta,
            event.gamma,
            event.alpha);
    }
}

function onClick() {
    console.log('requesting permission');

    if (typeof DeviceOrientationEvent.requestPermission === 'function') {
        DeviceOrientationEvent.requestPermission()
            .then(permissionState => {
                if (permissionState === 'granted') {
                    window.addEventListener("deviceorientation", handleOrientation, false);
                }
            })
            .catch(console.error);
    }
}

function toRadians(angle) {
    return angle * Math.PI / 180;
}

connection.on('rotated', (x, y, z) => {
    if (!planeMesh) return;

    planeMesh.rotation.x = toRadians(x);
    planeMesh.rotation.y = toRadians(y);
    planeMesh.rotation.z = toRadians(z);
});

connection.start().catch((err) => {
    console.error(err.toString());
});