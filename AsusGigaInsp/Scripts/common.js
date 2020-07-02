// フォームのクリア
function clearForm(form) {
    $(form)
        .find("input, select, textarea")
        .not(":button, :submit, :reset, :hidden")
        .val("")
        .prop("checked", false)
        .prop("selected", false)
        ;

    $(form).find(":radio").filter("[data-default]").prop("checked", true);
}

// 空判定（true：undefined, null, ""）
function isEmpty(ChkVal) {
    if (ChkVal || 0) {
        // 空で無い
        return false;
    } else {
        // 空
        return true;
    }
}