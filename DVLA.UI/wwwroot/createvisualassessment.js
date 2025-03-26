StartCam();

console.log('PassportImageUrl', document.getElementById('PassportImageUrl').value);
console.log('ContentLength', document.getElementById('PassportImageUrl').value.length);

document.getElementById("Image").addEventListener("change", function (event) {
    debugger
    const file = event.target.files[0]; // Get the selected file
    if (file) {
        const reader = new FileReader();

        reader.onload = function (e) {
            const preview = document.getElementById("passportpreview");
            preview.src = e.target.result; // Set the preview image source
            preview.style.display = "block"; // Show the preview image
            document.getElementById("PassportUploadType").value = "FileUpload";
        };

        reader.readAsDataURL(file); // Convert file to Data URL
       
    }
});

function load() {

    $('#PassResultId').empty();

    var id = $('#PassOrFail').val();
    var appId = $('#Id').val();
    if (appId != null) {
        if (id == 2 || id == null || id == '') {
            $('#PassResultId').empty()
            $('#PassResultId').hide()
            return false;
        }
        $('#PassResultId').show()
        $.ajax({
            type: "POST",
            url: "/VisualAssessmentResult/GetPassResult",
            datatype: "Json",
            data: { passOrFail: id },
            success: function (data) {
                debugger;

                $('#PassResultId').append('<option value="">Select pass type</option>');
                $.each(data, function (index, value) {
                    $('#PassResultId').append('<option value="' + value.value + '">' + value.text + '</option>');
                });
                var passOrFail = $('#PassOrFail').val();
                $('#PassResultId').val(passOrFail);
            }
        });
    }


}

function ChangeResultConclusion(e) {
    debugger;
    if (e.value == "Fit to drive" || e.value == "Fit to drive with glasses") {
        $("#PassOrFail").val("1");
        $("#PassResult").show();
        //load();
    } else if ((e.value = "Not fit to drive")) {
        $("#PassOrFail").val("2");
        $("#PassResult").hide();
    } else {
        $("#PassOrFail").val("");
        $("#PassResult").hide();
    }
}


function Capture() {
    let photo = document.getElementById("photo");
    let video = document.getElementById("video");
    let canvas = document.getElementById("canvas");
    // Draw the current frame from the video to the canvas
    const context = canvas.getContext("2d");
    context.drawImage(video, 0, 0, canvas.width, canvas.height);

    // Get the image data from the canvas as a data URL (base64 encoded image)
    const imageData = canvas.toDataURL("image/png");

    // Set the captured image as the source of the img element
    photo.src = imageData;

    //document.getElementById("PassportUrl").value = imageData;

    $("#PassportImageUrl").val(imageData);
    console.log(
        "PassportImageUrl",
        document.getElementById("PassportImageUrl").value
    );
    console.log(
        "Length",
        document.getElementById("PassportImageUrl").value.length
    );

    console.log("Image", imageData);

    // Display the captured image
    photo.style.display = "block";

    document.getElementById("PassportUploadType").value = "WebCam";
    

    //video.style.display = "none";
}



function StartCam() {
    // Access the webcam
    const video = document.getElementById("video");
    video.style.display = "block";

    const photo = document.getElementById("photo");
    photo.style.display = "block";

    //const canvas = document.getElementById("canvas");
    //canvas.style.display = "block";

    navigator.mediaDevices
        .getUserMedia({ video: true })
        .then((stream) => {
            video.srcObject = stream; // Set the stream to the video element
        })
        .catch((error) => {
            console.error("Error accessing webcam:", error);
        });
}



$(function () {
    $("#DOB").datepicker({
        dateFormat: "dd-mm-yyyy",
    });

    console.log(
        "PassportImageUrl",
        document.getElementById("PassportImageUrl").value
    );
    console.log(
        "Length",
        document.getElementById("PassportImageUrl").value.length
    );

    let currentYear = new Date().getFullYear();
    let currentMonth = new Date().getMonth();
    let currentDay = new Date().getDate();

    $("#DOB").on("changeDate", function (e) {
        var date = e.date;
        var day = ("0" + date.getDate()).slice(-2);
        var month = ("0" + (date.getMonth() + 1)).slice(-2);
        var year = date.getFullYear();
        $("#DateOfBirth").val(year + "-" + month + "-" + day);
    });



    $("#HX_BCV_OD").change(function () {
        if ($("#HX_BCV_OD").val() && $("#HX_BCV_OS").val()) {
            $("#HX_BCV_OU").val(
                parseInt($("#HX_BCV_OD").val()) + parseInt($("#HX_BCV_OS").val())
            );
        }
    });

    $("#HX_BCV_OS").change(function () {
        if ($("#HX_BCV_OS").val() && $("#HX_BCV_OD").val()) {
            $("#HX_BCV_OU").val(
                parseInt($("#HX_BCV_OD").val()) + parseInt($("#HX_BCV_OS").val())
            );
        }
    });

    $("#PassOrFail").change(function () {
        $("#PassResult").empty();

        var id = $("#PassOrFail").val();

        if (id == 2 || id == null || id == "") {
            $("#PassResult").empty();
            $("#PassResult").hide();
            return false;
        }
        $("#PassResult").show();
    });

    //$("#ResultServiceType").change(function () {
    //    var serviceType = $(this).val();
    //    if (serviceType == 1) {
    //        $("#LearnerType").css("display", "block");
    //        $("#LearnerDriversLicenceType").rules("add", {
    //            required: true,
    //            messages: "Learner Drivers Licence Type is required",
    //        });
    //    } else {
    //        $("#LearnerType").css("display", "none");
    //        $("#LearnerDriversLicenceType").rules("remove", "required");
    //    }
    //});

    let isSubmitted = false;

    $("#btnSave").click(function () {
        $("#ActionType").val("Modify");
        $("#Action").val("2");
        $("#target").submit();
        $(this).val("Submitting.. Please Wait..");
    });

    $("#btnSubmit").click(function () {
        $("#ActionType").val("Finalize");
        $("#Action").val("1");
        $("#target").submit();
        $(this).val("Submitting.. Please Wait..");
    });
});
