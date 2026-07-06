$(document).ready(function () {

    LoadCategories();
    ShowCourseData();

});

// MODAL AÇ
$('#btnAddCourse').click(function () {

    $('#CourseName').val('');
    $('#Description').val('');
    $('#Price').val('');
    $('#Duration').val('');
    $('#ImageUrl').val('');

    $('#CourseModal').removeData('id');

    var modal = new bootstrap.Modal(
        document.getElementById('CourseModal')
    );

    modal.show();
});

// LİSTELE
// LİSTELE
function ShowCourseData() {

    var url = $('#urlCourseData').val();
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
                html += '<td>' + item.categoryName + '</td>';
                html += '<td>' + item.courseName + '</td>';
                html += '<td>' + item.description + '</td>';
                html += '<td>' + item.price + '</td>';
                html += '<td>' + item.duration + '</td>';
                html += '<td class="no-print">' +
                    '<button class="btn btn-danger btn-sm me-1" onclick="Delete(' + item.id + ')">Sil</button>' +
                    '<button class="btn btn-warning btn-sm" onclick="Edit(' + item.id + ')">Düzenle</button>' +
                    '</td>';

                html += '</tr>';
            });

            $('#table_data').html(html);
        },
        error: function () {
            alert("Course verisi yüklenemedi");
        }
    });
}
// EKLE / UPDATE
function AddCourse() {

    var id = $('#CourseModal').data('id');

    var course = {
        CourseName: $('#CourseName').val(),
        Description: $('#Description').val(),
        Price: $('#Price').val(),
        Duration: $('#Duration').val(),
        ImageUrl: $('#ImageUrl').val(),
        CategoryId: $('#CategoryId').val()
    };

    if (course.CourseName === '') {
        alert("Kurs adı boş olamaz");
        return;
    }

    // UPDATE
    if (id) {

        course.Id = id;

        $.ajax({
            url: '/Course/Update',
            type: 'POST',
            data: course,
            success: function () {

                alert("Güncellendi");

                $('#CourseModal').removeData('id');
                HideModal();
                ShowCourseData();
            }
        });

        return;
    }

    // ADD
    $.ajax({
        url: '/Course/AddCourse',
        type: 'POST',
        data: course,
        success: function () {

            alert("Kurs eklendi");

            HideModal();
            ShowCourseData();
        }
    });
}

// EDİT
function Edit(id) {

    $.ajax({
        url: '/Course/Edit?id=' + id,
        type: 'GET',
        success: function (data) {

            $('#CourseName').val(data.courseName);
            $('#Description').val(data.description);
            $('#Price').val(data.price);
            $('#Duration').val(data.duration);
            $('#ImageUrl').val(data.imageUrl);
            $('#CategoryId').val(data.categoryId);

            var modal = new bootstrap.Modal(
                document.getElementById('CourseModal')
            );

            modal.show();

            $('#CourseModal').data('id', data.id);
        }
    });
}

// SİL
function Delete(id) {

    if (!confirm("Silmek istediğine emin misin?")) return;

    $.ajax({
        url: '/Course/Delete?id=' + id,
        type: 'GET',
        success: function () {

            alert("Silindi");
            ShowCourseData();
        }
    });
}

// MODAL KAPAT
function HideModal() {

    var modal = bootstrap.Modal.getInstance(
        document.getElementById('CourseModal')
    );

    if (modal) {
        modal.hide();
    }
}

// Arama kutusuna yazıldıkça listeyi güncelle
$("#txtSearch").keyup(function () {
    ShowCourseData();
});


function LoadCategories() {

    $.ajax({
        url: "/Course/CategoryList",
        type: "GET",
        success: function (data) {

            $("#CategoryId").empty();

            $("#CategoryId").append(
                '<option value="">Kategori Seçiniz</option>'
            );

            $.each(data, function (i, item) {

                $("#CategoryId").append(
                    '<option value="' + item.id + '">' +
                    item.categoryName +
                    '</option>'
                );

            });

        }
    });

}
