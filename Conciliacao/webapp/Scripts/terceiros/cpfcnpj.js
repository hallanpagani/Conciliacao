function mascaraCC(o,f){
    v_obj=o
    v_fun=f
    setTimeout('execmascara()',1)
}

function execmascara(){
    v_obj.value=v_fun(v_obj.value)
}

function CC(v){

    // Permite somente dígitos
    v=v.replace(/\D/g,'')

    if (v.length < 14) { // se tiver somente 14 caracteres, é um cpf

        //ponto entre o terceiro e o quarto dígito
        v=v.replace(/(\d{3})(\d)/,'$1.$2')

        /*ponto entre o terceiro e o quarto dígito
        para os próximo dígitos*/
        v=v.replace(/(\d{3})(\d)/,'$1.$2')

        //hífen entre o terceiro e o quarto dígito
        v=v.replace(/(\d{3})(\d{1,2})$/,'$1-$2')

    } else { // se tiver mais que 14 caracteres, é um cnpj

        //ponto entre o segundo e o terceiro dígito
        v=v.replace(/^(\d{2})(\d)/,'$1.$2')

        //ponto entre o quinto e o sexto dígito
        v=v.replace(/^(\d{2})\.(\d{3})(\d)/,'$1.$2.$3')

        //barra entre o oitavo e o nono dígito
        v=v.replace(/\.(\d{3})(\d)/,'.$1/$2')

        //hífen depois do bloco de quatro dígito
        v=v.replace(/(\d{4})(\d)/,'$1-$2') 
    }
    return v 
}