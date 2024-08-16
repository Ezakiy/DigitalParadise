function checkFields() {
    const fileInput = document.getElementById("fileInput");
    const submitChangesBtn = document.getElementById("submitChangesBtn");

    const isFileInputFilled = fileInput.files.length > 0;

    if (!isFileInputFilled) {
        submitChangesBtn.disabled = true;
    } else {
        submitChangesBtn.disabled = false;
    }
}

document.getElementById("fileInput").addEventListener("change", checkFields);


checkFields();

function displaySelectedImage(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            document.getElementById('userImage').setAttribute('src', e.target.result);
        }

        reader.readAsDataURL(input.files[0]);
    }
}
