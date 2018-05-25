function mascaraCEP(o, f) {
    v_obj = o
    v_fun = f
    setTimeout('execmascara()', 1)
}

function execmascara() {
    v_obj.value = v_fun(v_obj.value)
}

function CEP(v) {

    // Permite somente dígitos
    v = v.replace(/\D/g, '')

    if (v.length < 9) { // se tiver somente 14 caracteres, é um cpf

        //ponto entre o terceiro e o quarto dígito
        v = v.replace(/(\d{5})(\d)/, '$1-$2')

       

    } 
    return v
}