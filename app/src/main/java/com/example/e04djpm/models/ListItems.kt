package com.example.e04djpm.models

data class ListItems (
    var docId : String?,
    var name : String?,
    var owners : List<String>?) {

    constructor() : this(null,null,null)
}