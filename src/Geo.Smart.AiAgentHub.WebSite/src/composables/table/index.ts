import { ref } from 'vue';
import type { HeaderItem } from '@smart/vue-table';

const useTableSorting = (sortingKey: string) => {
    const sorting = ref<string>(sortingKey);
    const sortingDesc = ref<boolean | undefined>(true);

    const sortItems = (item: HeaderItem) => {
        sorting.value = item.key;
        sortingDesc.value = item.sortingDesc ?? undefined;
    };

    return {
        sorting,
        sortingDesc,
        sortItems,
    };
};

export default useTableSorting;
