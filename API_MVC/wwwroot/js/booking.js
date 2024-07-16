"columns": [
    { "data": "name", "width": "10%" }, 
    { "data": "address", "width": "10%" },
    { "data": "email", "width": "10%" },
    { "data": "phoneNumber", "width": "10%" },
    {
        "data": "bookingStatus",
        "render": function (data) {
            return data ? "Confirmed" : "Pending";
        },
        "className": "text-center"
    },
    { "data": "numberOfPerson", "width": "10%" },
    { "data": "selectedCategory", "width": "10%" },
    { "data": "count", "width": "10%" },
    {
        "data": "date",
        "render": function (data) {
            return new Date(data).toLocaleDateString();
        },
        "className": "text-center"
    },
    {
        "data": "id",
        "render": function (data) {
            return `
               <div class="text-center">
                  <a href="/Booking/Upsert/${data}" class="btn btn-info">
                      <i class="fas fa-edit"></i>
                  </a>
                  <button class="btn btn-danger" onclick="Delete('/Booking/Delete/${data}')">
                      <i class="fas fa-trash-alt"></i>
                  </button>
               </div>
            `;
        }
    }
]
