// Анимация при загрузке страницы
document.addEventListener('DOMContentLoaded', function () {
    // Добавляем класс fade-in для плавного появления
    const elements = document.querySelectorAll('.fade-in');
    elements.forEach((el, index) => {
        setTimeout(() => {
            el.style.opacity = '1';
            el.style.transform = 'translateY(0)';
        }, index * 100);
    });

    // Инициализация tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Обработка форм с подтверждением
    document.querySelectorAll('form[data-confirm]').forEach(form => {
        form.addEventListener('submit', function (e) {
            if (!confirm(this.getAttribute('data-confirm'))) {
                e.preventDefault();
            }
        });
    });
});

// Drag and drop функционал
function setupDragAndDrop() {
    const draggableCards = document.querySelectorAll('.draggable-card');
    const container = document.querySelector('.draggable-row');

    draggableCards.forEach(card => {
        card.addEventListener('dragstart', () => {
            card.classList.add('ghost');
            setTimeout(() => card.style.opacity = '0.4', 0);
        });

        card.addEventListener('dragend', () => {
            card.classList.remove('ghost');
            card.style.opacity = '1';
        });
    });

    if (container) {
        container.addEventListener('dragover', e => {
            e.preventDefault();
            const afterElement = getDragAfterElement(container, e.clientX);
            const draggable = document.querySelector('.ghost');
            if (draggable) {
                if (afterElement == null) {
                    container.appendChild(draggable);
                } else {
                    container.insertBefore(draggable, afterElement);
                }
            }
        });
    }
}

function getDragAfterElement(container, x) {
    const draggableElements = [...container.querySelectorAll('.draggable-card:not(.ghost)')];

    return draggableElements.reduce((closest, child) => {
        const box = child.getBoundingClientRect();
        const offset = x - box.left - box.width / 2;
        if (offset < 0 && offset > closest.offset) {
            return { offset: offset, element: child };
        } else {
            return closest;
        }
    }, { offset: Number.NEGATIVE_INFINITY }).element;
}

// Инициализация при загрузке
document.addEventListener('DOMContentLoaded', setupDragAndDrop); function selectRole(role) {
    const roleInput = document.getElementById("roleInput");
    if (roleInput) {
        roleInput.value = role;
    }

    document.querySelectorAll('.role-btn').forEach(btn => btn.classList.remove('selected'));
    const selectedBtn = document.getElementById(role.toLowerCase() + 'Btn');
    if (selectedBtn) {
        selectedBtn.classList.add('selected');
    }
}

document.addEventListener('DOMContentLoaded', () => {
    // Поддержка при перезагрузке
    const initialRole = document.getElementById("roleInput")?.value;
    if (initialRole) selectRole(initialRole);
});
