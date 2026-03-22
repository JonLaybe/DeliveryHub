export function formattedPrice(price: number) {
    const hasCents = price % 1 !== 0; // проверяем, есть ли копейки
    return price.toLocaleString('ru-RU', {
        style: 'currency',
        currency: 'RUB',
        minimumFractionDigits: hasCents ? 2 : 0,
        maximumFractionDigits: hasCents ? 2 : 0,
    });
}