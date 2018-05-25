function mascaraDT(o, f) {
    v_obj = o
    v_fun = f
    setTimeout('execmascara()', 1)
}

function execmascara() {
    v_obj.value = v_fun(v_obj.value)
}

function DATA(v) {

    // Permite somente dígitos
    v = v.replace(/\D/g, '')
    if (v.length < 10) {

        v = v.replace(/(\d{2})(\d)/, '$1/$2')
        v = v.replace(/(\d{2})(\d)/, '$1/$2')
    } 
    return v
}