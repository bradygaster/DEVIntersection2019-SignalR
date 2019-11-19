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

    subject.next({
        X: event.alpha,
        Y: event.beta,
        Z: event.gamma
    });
}

async function onClick() {
    console.log('requesting permission');
    if (typeof DeviceOrientationEvent.requestPermission === 'function') {
        DeviceOrientationEvent.requestPermission()
            .then(async (permissionState) => {
                if (permissionState === 'granted') {
                    subject = new signalR.Subject();
                    await connection.send("UploadStream", subject);
                    window.addEventListener("deviceorientation", handleOrientation, false);
                }
            })
            .catch(console.error);
    }
}

let subject;

async function onFakeClick() {
    var iteration = 0;
    subject = new signalR.Subject();

    await connection.send("UploadStream", subject);

    const intervalHandle = setInterval(() => {
        iteration++;
        console.log(iteration);
        subject.next({
            X: iteration,
            Y: iteration,
            Z: iteration
        });
        if (iteration === 360) {
            clearInterval(intervalHandle);
            subject.complete();
        }
    }, 10);
}

function toRadians(angle) {
    return angle * Math.PI / 180;
}

async function startDownloadStream() {
    console.log('starting download stream');
    connection.stream("DownloadStream").subscribe({
        next: (item) => {
            console.log('received x: ' + item.x + ', y: ' + item.y + ', z:' + item.z);
            planeMesh.rotation.x = toRadians(item.x);
            planeMesh.rotation.y = toRadians(item.y);
            planeMesh.rotation.z = toRadians(item.z);
        },
        complete: () => {
            console.log('stream completed');
        },
        error: (err) => {
            console.log('stream error: ' + err);
        },
    });
}

async function startConnection() {
    connection.start()
        .then(async () => {
            if (isControlPanel === false) {
                await startDownloadStream();
            }
        })
        .catch((err) => {
            console.error(err.toString());
        });
}


