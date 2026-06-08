export function formattedPrice(price: number) {
    const hasCents = price % 1 !== 0; // проверяем, есть ли копейки
    return price.toLocaleString('ru-RU', {
        style: 'currency',
        currency: 'RUB',
        minimumFractionDigits: hasCents ? 2 : 0,
        maximumFractionDigits: hasCents ? 2 : 0,
    });
}

export const getFormattedDate = (dateString: string | Date): string => {
    const date = new Date(dateString);
    if (isNaN(date.getTime())) return 'Некорректная дата';

    return date.toLocaleDateString();
};

// Получение даты в формате ГГГГ-ММ-ДД
export const getFormattedDateYMD = (date: Date): string => {
    if (isNaN(date.getTime())) return 'Некорректная дата';

    return new Intl.DateTimeFormat('en-CA', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit'
    }).format(date);
};