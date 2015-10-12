/**********************************************************************
This is a fix for handing Javascript month adding logic calculation
Uses example is below
var myDate = new Date("01/31/2012");
var result1 = newDate.addMonths(1); 
**********************************************************************/
Date.isLeapYear = function (year) {
    return (((year % 4 === 0) && (year % 100 !== 0)) || (year % 400 === 0));
};

Date.getDaysInMonth = function (year, month) {
    return [31, (Date.isLeapYear(year) ? 29 : 28), 31, 30, 31, 30, 31, 31, 30, 31, 30, 31][month];
};

Date.prototype.isLeapYear = function () {
    var y = this.getFullYear();
    return (((y % 4 === 0) && (y % 100 !== 0)) || (y % 400 === 0));
};

Date.prototype.getDaysInMonth = function () {
    return Date.getDaysInMonth(this.getFullYear(), this.getMonth());
};

Date.prototype.addMonths = function (value) {
    var n = this.getDate();
    this.setDate(1);
    this.setMonth(this.getMonth() + value);
    this.setDate(Math.min(n, this.getDaysInMonth()));
    return this;
};

/**********************************************************************
Grid related scripts
**********************************************************************/
Array.prototype.clear = function () {
    this.length = 0;
};

/* Shuffles the Array elements randomly */
Array.prototype.shuffle = function () {
    var i = this.length, j, t;
    while (i--) {
        j = Math.floor((i + 1) * Math.random());
        t = arr[i];
        arr[i] = arr[j];
        arr[j] = t;
    }
}

/* Removes redundant elements from the array */
Array.prototype.unique = function () {
    var a = [], i;
    this.sort();
    for (i = 0; i < this.length; i++) {
        if (this[i] !== this[i + 1]) {
            a[a.length] = this[i];
        }
    }
    return a;
}

/* Returns the index of the element matched from the behind */
Array.prototype.lastIndexOf = function (n) {
    var i = this.length;
    while (i--) {
        if (this[i] === n) {
            return i;
        }
    }
    return -1;
}

Array.prototype.contains = function (element) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] == element) {
            return true;
        }
    }
    return false;
};

Array.prototype.addUnique = function (value) {
    if (value instanceof Array) {
        for (var i = 0; i < value.length; i++) {
            var found = false;
            for (var j = 0; j < this.length; j++) {
                if (value[i] == this[j]) {
                    found = true;
                    break;
                }
            }
            if (!found)
                this.push(value[i]);
        }
    }
    else {
        var found = false;
        for (var i = 0; i < this.length; i++) {
            if (this[i] == value) {
                found = true;
                break;
            }
        }
        if (!found)
            this.push(value);
    }
};

Array.prototype.remove = function (value) {
    var newItems = Array();
    if (value instanceof Array) {
        for (var i = 0; i < this.length; i++) {
            var found = false;
            for (var j = 0; j < value.length; j++) {
                if (this[i] == value[j]) {
                    found = true;
                    break;
                }
            }
            if (!found)
                newItems.push(this[i]);
        }
    }
    else {
        for (var i = 0; i < this.length; i++) {
            if (this[i] != value)
                newItems.push(this[i]);
        }
    }
    this.clear();
    for (var i = 0; i < newItems.length; i++) {
        this.push(newItems[i]);
    }
}