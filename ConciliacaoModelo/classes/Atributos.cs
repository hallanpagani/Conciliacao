using System;

namespace ConciliacaoModelo.classes
{
    // atributo de campo autoinc
    public class AutoIncAttribute : Attribute
    {
    }

    // atributo que indica campo somente para inserção - não entra no insert
    public class OnlyInsertAttribute : Attribute
    {
    }

    // atributo que indica campo somente para inserção - não entra no insert
    public class OnlySelectAttribute : Attribute
    {
    }

    // atributo que indica campo somente para update - não entra na inserção
    public class OnlyUpdateAttribute : Attribute
    {
    }
}