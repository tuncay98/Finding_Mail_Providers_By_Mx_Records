async function GetData(url = '', data = {}) {
    const response = await fetch(url, {
        method: 'GET'
    });

    return await response.json();
}

var input = document.querySelector(".customMxInput");
var text = document.querySelector(".spanInputText");
var button = document.querySelector(".searchBtn");
var bar = document.querySelector(".loadingio-spinner-pulse-7vvqe6ujdap");
var form = document.querySelector("#form");

var nameOfFile = "";

input.addEventListener("change", function (e) {
    if (e.target.files[0] != null)
        text.innerHTML = e.target.files[0].name
    else
        text.innerHTML = "File not chosen"

    nameOfFile = e.target.files[0].name
});

button.addEventListener("click", function () {
    if (text.innerHTML != "File not chosen")
        bar.style.display = "inline-block";
    else
        bar.style.display = "none"
})

var id = '_' + Math.random().toString(36).substr(2, 6);

var time = new Date().toUTCString();

var link = "/Home/SaveEnteredTime?date=" + time + "&session=" + id;

GetData(link).then((data) => {
    if (data == "Done") {
        console.log("EnteredTime Done")
    } else {
        console.log("EnteredTime Error")
    }
})


form.addEventListener("submit", function (e) {

    var link = "/Home/SaveFile?filename=" + nameOfFile;

    GetData(link).then((data) => {
        if (data == "Done") {
            console.log("EnteredTime Done")
        } else {
            console.log("EnteredTime Error")
        }
    })


})
