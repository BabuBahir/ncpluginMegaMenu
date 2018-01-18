$(document).ready(function () {

        $('#Product ,#Category ,  #Manufacturer').click(function () {
            var get_idname = $(this).attr("id");
            var get_checkedval = $(this).is(":checked");   //  true or false

            switch (get_idname) {
                case "Product": Fn_Show_Product_image(get_checkedval); break;
                case "Category": Fn_Show_Category_image(get_checkedval); break;
                case "Manufacturer": Fn_Show_Manufacturer_image(get_checkedval); break;
            }
        });

        function Fn_Show_Product_image(get_checkedval) {
            if (get_checkedval == true) {
                $(".Show_Product_image").show();
            } else {
                $(".Show_Product_image").hide();
            }
        };

        function Fn_Show_Category_image(get_checkedval) {
            if (get_checkedval == true) {
                $(".Show_Category_image").show();
            } else {
                $(".Show_Category_image").hide();
            }
        };

        function Fn_Show_Manufacturer_image(get_checkedval) {
            if (get_checkedval == true) {
                $(".Show_Manufacturer_image").show();
            } else {
                $(".Show_Manufacturer_image").hide();
            }
        }


})