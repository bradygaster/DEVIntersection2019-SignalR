var connection = new signalR.HubConnectionBuilder()
    .withUrl('/process')
    .build();

connection.on('dashboardUpdated', (step) => {
    console.log(step + ' completed');
    document.getElementById(step).classList.remove('bg-secondary');
    document.getElementById(step).classList.add('bg-success');
});

async function startProcess() {
    for (i = 0; i < document.getElementsByClassName('card').length; i++) {
        document.getElementsByClassName('card')[i].classList.add('bg-secondary');
        document.getElementsByClassName('card')[i].classList.remove('bg-success');
    }

    connection.invoke('StartProcess');
}

async function start() {
    await connection.start();
}

start();