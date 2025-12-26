import { defineStore } from 'pinia';
import { ref } from 'vue';

import { apiProfileMe } from '@/api';
import usePageStore from '@/stores/page';

interface Profile {
    [key: string]: unknown;
}

const useProfileStore = defineStore('profile', () => {
    const { setIsLoading } = usePageStore();
    const profile = ref<Profile | null>(null);
    const photoUrl = ref(null);

    const setProfile = (val: Profile | null): void => {
        profile.value = val;
    };

    const getProfile = async () => {
        try {
            setIsLoading(true);
            const { data } = await apiProfileMe();
            if (!data.success) {
                setProfile(null);
                return;
            }
            setProfile(data.data);
        } finally {
            setIsLoading(false);
        }
    };

    return {
        profile,
        photoUrl,
        setProfile,
        getProfile,
    };
});

export default useProfileStore;
