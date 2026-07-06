$(document).ready(function () {
    LoadCourses();
    ShowEnrollmentData();
});
// MODAL AÇ
$('#btnAddEnrollment').click(function () {

    $('#StudentName').val('');
    $('#Email').val('');
    $('#Phone').val('');
    $('#CourseId').val('');

    $('#EnrollmentModal').removeData('id');

    new bootstrap.Modal(
        document.getElementById('EnrollmentModal')
    ).show();
});

// LİSTELE
// LİSTELE
function ShowEnrollmentData() {

    var url = $('#urlEnrollmentData').val();
    var search = $('#txtSearch').val(); // <-- Eklendi: Arama kutusundaki değer alınıyor

    $.ajax({
        url: url,
        type: 'GET',
        data: { search: search }, // <-- Eklendi: Arama parametresi sunucuya gönderiliyor
        dataType: 'json',
        success: function (result) {

            var html = '';

            $.each(result, function (index, item) {

                html += '<tr>';
                html += '<td>' + item.studentName + '</td>';
                html += '<td>' + item.email + '</td>';
                html += '<td>' + item.phone + '</td>';
                html += '<td>' + item.enrollmentDate + '</td>';
                html += '<td>' + item.courseName + '</td>';

                html += '<td class="no-print">' +
                    '<button class="btn btn-danger btn-sm me-1" onclick="Delete(' + item.id + ')">Sil</button>' +
                    '<button class="btn btn-warning btn-sm" onclick="Edit(' + item.id + ')">Düzenle</button>' +
                    '</td>';

                html += '</tr>';
            });

            $('#table_data').html(html);
        },
        error: function () {
            alert("Enrollment verisi yüklenemedi");
        }
    });
}
function SaveEnrollment() {

    var id = $('#EnrollmentModal').data('id');

    var obj = {
        Id: id,
        StudentName: $('#StudentName').val(),
        Email: $('#Email').val(),
        Phone: $('#Phone').val(),
        CourseId: $('#CourseId').val()
    };

    if (!obj.StudentName || !obj.Email || !obj.Phone || !obj.CourseId) {
        alert("Lütfen tüm alanları doldur");
        return;
    }

    // UPDATE
    if (id) {

        $.ajax({
            url: '/Enrollment/Update',
            type: 'POST',
            data: obj,
            success: function () {

                alert("Güncellendi");

                $('#EnrollmentModal').removeData('id');
                HideModal();
                ShowEnrollmentData();
            }
        });

        return;
    }

    // ADD
    $.ajax({
        url: '/Enrollment/AddEnrollment',
        type: 'POST',
        data: obj,
        success: function () {

            alert("Kayıt eklendi");

            HideModal();
            ShowEnrollmentData();
        }
    });
}

// EKLE / UPDATE
function AddEnrollment() {

    var obj = {
        StudentName: $('#StudentName').val(),
        Email: $('#Email').val(),
        Phone: $('#Phone').val(),
        CourseId: $('#CourseId').val()
    };

    $.ajax({
        url: '/Enrollment/AddEnrollment',
        type: 'POST',
        data: obj,
        success: function () {
            alert("Kayıt eklendi");
            ShowEnrollmentData();
            $('#EnrollmentModal').modal('hide');
        }
    });
}

// EDİT
function Edit(id) {

    $.ajax({
        url: '/Enrollment/Edit?id=' + id,
        type: 'GET',
        success: function (data) {

            $('#StudentName').val(data.studentName);
            $('#Email').val(data.email);
            $('#Phone').val(data.phone);
            $('#CourseId').val(data.courseId);

            $('#EnrollmentModal').data('id', data.id);

            new bootstrap.Modal(
                document.getElementById('EnrollmentModal')
            ).show();
        }
    });
}
// SİL
function Delete(id) {

    if (!confirm("Silmek istediğine emin misin?")) return;

    $.ajax({
        url: '/Enrollment/Delete?id=' + id,
        type: 'GET',
        success: function () {

            alert("Silindi");
            ShowEnrollmentData();
        }
    });
}

// MODAL KAPAT
function HideModal() {

    var modal = bootstrap.Modal.getInstance(
        document.getElementById('EnrollmentModal')
    );

    if (modal) {
        modal.hide();
    }
}

function LoadCourses() {

    $.ajax({
        url: '/Enrollment/CourseList',
        type: 'GET',
        success: function (data) {

            $('#CourseId').empty();
            $('#CourseId').append('<option value="">Kurs Seçiniz</option>');

            $.each(data, function (i, item) {
                $('#CourseId').append(
                    '<option value="' + item.id + '">' + item.courseName + '</option>'
                );
            });
        }
    });
}
// Arama kutusuna yazıldıkça listeyi güncelle
$("#txtSearch").keyup(function () {
    ShowEnrollmentData();
});