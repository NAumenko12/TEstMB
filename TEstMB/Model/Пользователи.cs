//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TEstMB.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Пользователи
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Пользователи()
        {
            this.Заявки = new HashSet<Заявки>();
            this.Заявки1 = new HashSet<Заявки>();
            this.Комментарии = new HashSet<Комментарии>();
        }
    
        public int ID_Пользователя { get; set; }
        public string ФИО { get; set; }
        public string Номер_телефона { get; set; }
        public string Логин { get; set; }
        public string Пароль { get; set; }
        public Nullable<int> FK_Роли { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Заявки> Заявки { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Заявки> Заявки1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Комментарии> Комментарии { get; set; }
        public virtual Роли Роли { get; set; }
    }
}
