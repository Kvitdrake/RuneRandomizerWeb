// Сортировка вопросов
$(document).on('click', '.btn-move-up', function () {
    const $card = $(this).closest('.question-card');
    $card.insertBefore($card.prev());
    saveOrder();
});

function saveOrder() {
    const order = [];
    $('#questions .question-card').each((index, el) => {
        order.push({ id: $(el).data('id'), order: index });
    });

    $.post($('#questions').data('save-order-url'), { items: order });
}