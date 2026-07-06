$(document).ready(function () {
    ShowCategoryData();
});

// MODAL AÇ
$('#btnAddCategory').click(function () {

    $('#CategoryName').val('');

    var modal = new bootstrap.Modal(
        document.getElementById('CategoryModal')
    );

    modal.show();
});

// LİSTELE
// LİSTELE
function ShowCategoryData() {

    var url = $('#urlCategoryData').val();
    var search = $('#txtSearch').val();

    $.ajax({
        url: url,
        type: 'GET',
        data: { search: search }, // <-- SUNUCUYA ARAMA PARAMETRESİNİ GÖNDEREN KISIM
        dataType: 'json',
        success: function (result) {

            var html = '';

            $.each(result, function (index, item) {

                html += '<tr>';
                html += '<td>' + item.categoryName + '</td>';

                html += '<td class="no-print">' +
                    '<button class="btn btn-danger btn-sm me-1" onclick="Delete(' + item.id + ')">Sil</button>' +
                    '<button class="btn btn-warning btn-sm" onclick="Edit(' + item.id + ')">Düzenle</button>' +
                    '</td>';

                html += '</tr>';
            });

            $('#table_data').html(html);
        },
        error: function () {
            alert("Kategori verisi yüklenemedi");
        }
    });
}
// EKLE
function AddCategory() {

    var name = $('#CategoryName').val();
    var id = $('#CategoryModal').data('id');

    if (name === '') {
        alert("Kategori adı boş olamaz");
        return;
    }

    // UPDATE
    if (id) {

        $.ajax({
            url: '/Category/Update',
            type: 'POST',
            data: { Id: id, CategoryName: name },
            success: function () {

                alert("Güncellendi");

                $('#CategoryName').val('');
                $('#CategoryModal').removeData('id');

                ShowCategoryData();
                HideModal();
            }
        });

        return;
    }

    // ADD
    $.ajax({
        url: '/Category/AddCategory',
        type: 'POST',
        data: { CategoryName: name },
        success: function () {

            alert("Kategori eklendi");

            $('#CategoryName').val('');
            ShowCategoryData();
            HideModal();
        }
    });
}

// EDİT
function Edit(id) {

    $.ajax({
        url: '/Category/Edit?id=' + id,
        type: 'GET',
        success: function (data) {

            $('#CategoryName').val(data.categoryName);

            var modal = new bootstrap.Modal(
                document.getElementById('CategoryModal')
            );

            modal.show();

            // update moduna geçmek için id sakla
            $('#CategoryModal').data('id', data.id);
        }
    });
}

// SİL
function Delete(id) {

    if (!confirm("Silmek istediğine emin misin?")) return;

    $.ajax({
        url: '/Category/Delete?id=' + id,
        type: 'GET',
        success: function () {

            alert("Silindi");
            ShowCategoryData();
        },
        error: function () {
            alert("Silme hatası");
        }
    });
}

// MODAL KAPAT
function HideModal() {

    var modal = bootstrap.Modal.getInstance(
        document.getElementById('CategoryModal')
    );

    if (modal) {
        modal.hide();
    }
}

$("#txtSearch").keyup(function () {
    ShowCategoryData();
});