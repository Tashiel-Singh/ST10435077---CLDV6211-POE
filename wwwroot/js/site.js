// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function showLoading() {
    document.getElementById('loadingSpinner').style.display = 'block';
}

function confirmDelete(venueName) {
    return confirm(`Are you sure you want to delete ${venueName}?`);
}

function validateImageUpload(input) {
    const maxSize = 5 * 1024 * 1024; // 5MB
    const allowedTypes = ['image/jpeg', 'image/png', 'image/gif'];
    
    if (input.files[0].size > maxSize) {
        alert('File is too large. Maximum size is 5MB.');
        input.value = '';
        return false;
    }
    
    if (!allowedTypes.includes(input.files[0].type)) {
        alert('Invalid file type. Please upload JPG, PNG or GIF.');
        input.value = '';
        return false;
    }
    
    showLoading();
    return true;
}
